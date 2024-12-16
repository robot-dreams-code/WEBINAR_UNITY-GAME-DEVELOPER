#ifndef _GRASSUTILITY_H_
#define _GRASSUTILITY_H_

struct DrawData
{
    float4x4 objectToWorld;
    float4 color;
};

StructuredBuffer<DrawData> _DrawData;

void setup()
{
    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    DrawData drawData = _DrawData[unity_InstanceID];
  
    unity_ObjectToWorld = mul(unity_ObjectToWorld, drawData.objectToWorld);

    float3x3 w2oRotation;
    w2oRotation[0] = unity_ObjectToWorld[1].yzx * unity_ObjectToWorld[2].zxy - unity_ObjectToWorld[1].zxy * unity_ObjectToWorld[2].yzx;
    w2oRotation[1] = unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[2].yzx - unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[2].zxy;
    w2oRotation[2] = unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[1].zxy - unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[1].yzx;

    float det = dot(unity_ObjectToWorld[0].xyz, w2oRotation[0]);
    w2oRotation = transpose(w2oRotation);
    w2oRotation *= rcp(det);
    float3 w2oPosition = mul(w2oRotation, -unity_ObjectToWorld._14_24_34);

    unity_WorldToObject._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
    unity_WorldToObject._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
    unity_WorldToObject._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
    unity_WorldToObject._14_24_34_44 = float4(w2oPosition, 1.0f);
    #endif
}

void GetPosition_float(float3 position, out float3 outPosition)
{
    outPosition = position;
}

/*void GetInstanceId_float(out float instanceId)
{
    instanceId = unity_InstanceID;
}*/

void GetInstanceData_float(float instanceId, out float4x4 instanceToWorld, out float4 instanceColor)
{
    DrawData drawData = _DrawData[instanceId];
    instanceToWorld = drawData.objectToWorld;
    instanceColor = drawData.color;
}

#endif