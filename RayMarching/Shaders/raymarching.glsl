#version 430 core

layout(binding = 0, rgba32f) uniform writeonly image2D framebuffer;

uniform vec3 eye;
uniform vec3 direction;
uniform float time;

#define EPSILON 0.00001
#define PI 3.1415926
#define MAX_DEPTH 1000

struct Sphere
{
    vec3 pos;
    float radius;
    vec3 color;
};
struct Box
{
    vec3 p;
    vec3 b;
    vec3 color;
};

const Sphere spheres[] =
{
    {vec3(-6, 0, 4), 1, vec3(163 / 255.0, 68 / 255.0, 0)},
    {vec3(-4, 1, 4), 1, vec3(163 / 255.0, 68 / 255.0, 0)},
    {vec3(-3, 0, 4), 1, vec3(31 / 255.0, 81 / 255.0, 167 / 255.0)}
};

layout(local_size_x = 8, local_size_y = 8) in;

// Distance functions
float sphere(vec3 pos, vec3 center, float r)
{
    return length(pos - center) - r;
}

float smin(float a, float b, float k)
{
    float h = max(k - abs(a-b), 0);
    return min(a, b) - h*h/(k*4);
}

/* row of spheres that are smoothed together */
float smoothedRow(vec3 pos, float r)
{
    return smin(
            sphere(pos, vec3(3, 0, 4), r),
            smin(
                sphere(pos, vec3(4, 0, 4),  r),
                sphere(pos, vec3(5, 0, 4),  r),
                0.1
                ),
                0.1
            );
}

float smoothedSpheres(vec3 pos, float r)
{
    float smoothFactor = abs(sin(time));
    return smin(
            sphere(pos, vec3(4, 3, 4), r),
            sphere(pos, vec3(3, 3, 4), r),
            smoothFactor);
}

float morphingSpheres(vec3 pos, float r)
{
    float off = sin(time);
    return smin(
            sphere(pos, vec3(off, 0, 4), r),
            sphere(pos, vec3(0, 0, 4), r),
            0.1);
}

/* returns distance and material index */
vec2 sceneSDF(vec3 p)
{
    float dist = MAX_DEPTH;
    float matIndex;
    // bunch of hardcoded spheres that don't move
    for (int i = 0; i < spheres.length; i++)
    {
        float d = sphere(p, spheres[i].pos, spheres[i].radius);
        if (d < dist)
        {
            dist = d;
            matIndex = 1.0;
        }
    }
    float d = smoothedRow(p, 1);
    if (d < dist)
    {
        dist = d;
        matIndex = 2.0;
    }
    d = morphingSpheres(p, 1);
    if (d < dist)
    {
        dist = d;
        matIndex = 3.0;
    }
    d = smoothedSpheres(p, 1);
    if (d < dist)
    {
        dist = d;
        matIndex = 4.0;
    }
    return vec2(dist, matIndex);
}

vec2 march(vec3 pos, vec3 dir)
{
    float depth = 0.0;
    float matIndex = 0;
    const int maxMarchingSteps = 100;
    for (int i = 0; i < maxMarchingSteps; i++)
    {
        vec2 res = sceneSDF(pos + depth * dir);
        float dist = res.x;
        matIndex = res.y;
        if (dist < EPSILON)
        {
            return vec2(depth, matIndex);
        }
        depth += dist;
        if (depth >= MAX_DEPTH)
        {
            return vec2(MAX_DEPTH, -1.0);
        }
    }
    return vec2(depth, matIndex);
}

/*
 * given a screenspace position
 * this will calculate the correct ray based
 * on the world view direction
 */
vec3 getDirection(ivec2 pixel, ivec2 size)
{
    float fov = 140;
    float aspect = size.x / float(size.y);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), direction)) * aspect;
    vec3 up = normalize(cross(direction, right));
    float fovAdjust = 1.0 / tan(fov / 2.0 * (PI / 180.0));
    float xoff = -1.0 + ((float(pixel.x) / float(size.x - 1.0)) * 2.0);
    float yoff = 1.0 - ((float(pixel.y) / float(size.y - 1.0)) * 2.0);
    return direction + (xoff * fovAdjust * right) + (yoff * fovAdjust * up);
}

vec3 sdfNormal(vec3 pos)
{
    // https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
    // uses gradient to estimate surface normal
    // expensive but works on any kind of surface
    vec2 e = vec2(1.0, -1.0) * 0.5773 * 0.001;
    return normalize(e.xyy * sceneSDF(pos + e.xyy).x + 
                     e.yyx * sceneSDF(pos + e.yyx).x + 
                     e.yxy * sceneSDF(pos + e.yxy).x + 
                     e.xxx * sceneSDF(pos + e.xxx).x);
}

vec3 light(vec3 eye, vec3 sdfPos, vec3 lightPos, vec3 lightPower, float illumination)
{
    vec3 n = sdfNormal(sdfPos);
    vec3 l = normalize(lightPos - sdfPos);
    vec3 v = normalize(eye - sdfPos);
    vec3 r = normalize(reflect(-l, n));

    float dotLN = dot(l, n);
    if (dotLN < 0)
    {
        // light not visible
        return vec3(0);
    }

    float dotRV = dot(r, v);
    if (dotRV < 0)
    {
        // reflection away from eye -> diffuse only
        return lightPower * dotLN;
    }
    return lightPower * (dotLN + pow(dotRV, illumination));
}

vec3 phong(vec3 eye, vec3 sdfPos, vec3 dir)
{
    const vec3 ambient = vec3(0.2);
    // hardcode lights
    
    float frenel = clamp(1 + dot(dir, sdfNormal(sdfPos)), 0, 1);

    return ambient +
        light(eye, sdfPos, vec3(0, 2, -2), vec3(0.5), 10) +
        frenel * light(eye, sdfPos, vec3(0, -2, -2), vec3(0.1), 20);
}

void main()
{
    ivec2 pos = ivec2(gl_GlobalInvocationID.xy);
    ivec2 size = imageSize(framebuffer);
    // discard points outside texture
    if (pos.x >= size.x || pos.y >= size.y)
    {
        return;
    }

  
    vec3 dir = getDirection(pos, size);

    vec3 col = vec3(0);
    vec2 res = march(eye, dir);
    float distToScene = res.x;
    float matIndex = res.y;
    if (distToScene >= MAX_DEPTH)
    {
        col = vec3(0,148/255.0,1);
    }
    else
    {
        if (matIndex < 0.5)
        {
            col = vec3(0); // reserved/unused
        }
        else if (matIndex < 1.5)
        {
            col = vec3(0.4545, 0, 0.2); // individual spheres
        }
        else if (matIndex < 2.5)
        {
            col = vec3(0.1, 0.2, 0.9); // smoothedRow
        }
        else if (matIndex < 3.5)
        {
            col = vec3(0.45, 0.1, 0.33); // morphingSpheres
        }
        else if (matIndex < 4.5)
        {
            col = vec3(0.1, 0.9, 0.1); // smoothedSpheres
        }
        else
        {
            // undefined
            col = vec3(0);
        }

        vec3 scenePos = eye + dir * distToScene;
        col += phong(eye, scenePos, dir);

        col = pow(col, vec3(0.8,0.9,1.0));
    }
    imageStore(framebuffer, pos, vec4(col, 1));
}
