Shader "Custom/Draw"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _LP1 ("LinePoint1", Vector) = (0,0,0,0)
        _LP2 ("LinePoint2", Vector) = (0,0,0,0)
        _LineWidth ("LineWidth", Range(1,20)) = 1
        _MainTex ("Texture", 2D) = "white" {}

        _StencilComp ("Stencil Comparison", Float) = 8.000000
        _Stencil ("Stencil ID", Float) = 0.000000
        _StencilOp ("Stencil Operation", Float) = 0.000000
        _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
        _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
        _ColorMask ("Color Mask", Float) = 15.000000
    }
    
    SubShader
    {
        Tags {"Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Stencil {
            Ref [_Stencil]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _LP1;
            float4 _LP2;
            float4 _Color;
            float _LineWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                /*if(pow((i.pos.x - _LP1.x),2) + pow((i.pos.y - _LP1.y),2) < 300)
                {
                    return fixed4(0,0,1,1);
                }

                if(pow((i.pos.x - _LP2.x),2) + pow((i.pos.y - _LP2.y),2) < 300)
                {
                    return fixed4(0,0,1,1);
                }*/

                float d = abs((_LP2.y - _LP1.y) * i.pos.x + (_LP1.x - _LP2.x) * i.pos.y + _LP2.x * _LP1.y - _LP2.y * _LP1.x) / sqrt(pow(_LP2.y - _LP1.y,2) + pow(_LP2.x - _LP1.x,2));

                if(d <= _LineWidth / 2)
                {
                    return _Color;
                }

                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
