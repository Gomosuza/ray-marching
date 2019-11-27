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
    {vec3(-3, 1, 0), 1, vec3(163 / 255.0, 68 / 255.0, 0)},
    {vec3(0, 1, 0), 1, vec3(163 / 255.0, 68 / 255.0, 0)},
    {vec3(1, 1, 0), 1, vec3(54 / 255.0, 210 / 255.0, 21 / 255.0)},
    {vec3(2, 1, 0), 1, vec3(31 / 255.0, 81 / 255.0, 167 / 255.0)}
};

layout(local_size_x = 8, local_size_y = 8) in;

// Distance functions
float sphere(vec3 pos, vec3 center, float r)
{
    return length(pos - center) - r;
}

float box(vec3 p, vec3 b)
{
    vec3 q = abs(p) - b;
    return length(max(q,vec3(0))) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sceneSDF(vec3 p)
{
    float dist = MAX_DEPTH;
    for (int i = 0; i < spheres.length; i++)
    {
        float d = sphere(p, spheres[i].pos, spheres[i].radius);
        if (d < dist)
            dist = d;
    }
    return dist;
}

float march(vec3 pos, vec3 dir)
{
    float depth = 0.0;
    const int maxMarchingSteps = 100;
    for (int i = 0; i < maxMarchingSteps; i++)
    {
        float dist = sceneSDF(pos + depth * dir);
        if (dist < EPSILON)
        {
            return depth;
        }
        depth += dist;
        if (depth >= MAX_DEPTH)
        {
            return MAX_DEPTH;
        }
    }
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

vec3 sdfNormal(vec3 p)
{
    // use gradient to estimate surface normal
    // expensive, works on any kind of surface
    return normalize(vec3(
        sceneSDF(vec3(p.x + EPSILON, p.y, p.z)) - sceneSDF(vec3(p.x - EPSILON, p.y, p.z)),
        sceneSDF(vec3(p.x, p.y + EPSILON, p.z)) - sceneSDF(vec3(p.x, p.y - EPSILON, p.z)),
        sceneSDF(vec3(p.x, p.y, p.z  + EPSILON)) - sceneSDF(vec3(p.x, p.y, p.z - EPSILON))
    ));
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

vec3 phong(vec3 eye, vec3 sdfPos)
{
    const vec3 ambient = vec3(0.2);
    // hardcode single light
    return ambient +
    light(eye, sdfPos, vec3(0, 2, -2), vec3(0.5), 10) +
    light(eye, sdfPos, vec3(0, -2, -2), vec3(0.1), 20);
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
    float distToScene = march(eye, dir);
    if (distToScene >= MAX_DEPTH)
    {
        col = vec3(0,148/255.0,1);
    }
    else
    {
        // fade objects based on distance
        col = vec3(1 - (distToScene / 5.0));

        vec3 scenePos = eye + dir * distToScene;
        col = phong(eye, scenePos);
    }
    imageStore(framebuffer, pos, vec4(col, 1));
}
