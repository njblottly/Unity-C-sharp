Shader "Duck Adventure/Marsh Hit Flash"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite",2D)="white"{}
        _Color("Tint",Color)=(1,1,1,1)
        [HideInInspector] _RendererColor("Renderer Color",Color)=(1,1,1,1)
        [HideInInspector] _Flip("Flip",Vector)=(1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha",2D)="white"{}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha",Float)=0
        [PerRendererData] _HitFlash("Hit flash",Range(0,1))=0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
        Cull Off Lighting Off ZWrite Off Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment MarshFrag
            #pragma multi_compile_instancing
            #include "UnitySprites.cginc"
            float _HitFlash;
            fixed4 MarshFrag(v2f input):SV_Target
            {
                fixed4 color=SampleSpriteTexture(input.texcoord)*input.color;
                color.rgb=lerp(color.rgb,fixed3(1,1,1),_HitFlash);
                color.rgb*=color.a;
                return color;
            }
            ENDCG
        }
    }
}

