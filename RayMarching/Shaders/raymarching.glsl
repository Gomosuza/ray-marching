#version 430 core

layout(binding = 0, rgba32f) uniform writeonly image2D framebuffer;

uniform vec3 eye;
uniform vec3 direction;
uniform float time;

#define EPSILON 0.00001
#define PI 3.1415926
#define MAX_DEPTH 1000

layout(local_size_x = 8, local_size_y = 8) in;

// Distance functions
float sphere(vec3 pos, vec3 center, float r)
{
    return length(pos - center) - r;
}

float sceneSDF(vec3 p)
{
    // scene = min of all objects. CSG is built with nested min/max calls
    return min(
        min(
            sphere(p, vec3(0,2,0), 1),
            sphere(p, vec3(0,1,0), 1)
        ),
        sphere(p, vec3(2,0,0), 1)
    );
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
    }
    imageStore(framebuffer, pos, vec4(col, 1));
}
