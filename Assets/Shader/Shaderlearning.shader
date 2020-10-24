Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Color("Color" , Color) = (1,1,1,0)
        _Gloss("Gloss" , float) = 1
        _ShoelineTex ("Shoeline", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            // Files below include macros and functions to assist
			// with lighting and shadows.
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            //Mesh data : like vertex position,vertex normal,UVs,tangents,vertexs colour
            // this struct can name anything instead of appdata i going to write Vertex Input

            // In this mesh data this schemics has spq
            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            //Delcaring the varible for the properties above
            uniform float4 _Color;
            uniform Float _Gloss;
            uniform float3 _Mousepos;


            // This is the output of the vertex shader 
            // After get process by the vertex shader it will get the output over her
            // this struct can name anything instead of v2f I am going to write VertexOutput
            // the value in struct is matter not there name like "float2 uv :TEXCOORD0" the word "TEXCOORD0" will give us the value


            struct VertexOutput
            {
                //SV_POSITION will read as a clip position in the shader
                //The can also change the variable name "vertex" . Here i am using 

                float4 clipSpacePos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 worldpos : TEXCOORD2;

            };

            sampler2D _ShoelineTex;
            float4 _MainTex_ST;
           
            //Vertex Shader
            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;

                //This v.normal is "Interpolated " vertor so the lenght of this verter might be less that the 1
                o.normal =v.normal;
                
                o.worldpos = mul(unity_ObjectToWorld,v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv0, _MainTex); 
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // value = lerp(a,b,t)
            // t = inverlerp (a,b,t);


            float3 Mylerp (float3 a,float3 b,float t)
            {
                // a+(b-a)*t
                
                return t*b+(1.0-t)*a;
                
            }

            //There no inverlerp in CG but it kind of version is presenet in CG
            // i.e SmoothStep
            float3 Inverlerp (float3 a, float3 b,float3 value)
            {
               // t = (value - a) / (b - a);
                return (max(0,(value -a)/(b-a)));
            }
            
            //This Fuction give strip thing
            float Poaterize(float step , float value)
            {  
                return floor(value*step)/step;
            }


            //Fragment shader
            fixed4 frag (VertexOutput o) : SV_Target
            {
                float3 ColorA = float3 (0.1,0.8,1);
                float3 ColorB = float3 (1,1,1);

                // frac equation (value - floor(value))
                //return frac(_Time.y);
                float shorelineMap = tex2D(_ShoelineTex, o.uv0).x;
                //return float4(shorelineMap,0,0,0);
                float shape = shorelineMap;
                float wavesize = 0.04;
                float waveamp = (sin(shape/wavesize+_Time.y*4));
                waveamp *= shorelineMap;
            

                return waveamp;

                 //return(((sin(shorelineMap*16 + _Time.y)+1)*0.5));
                
                
            
                float3 distcolor ;
                float dist = distance(_Mousepos,o.worldpos);
                dist = Poaterize(8,dist);
                distcolor = lerp(ColorA,ColorB,dist);
                if(dist>1)
                {
                    distcolor = float3(1,1,1);
                } 
            
                return float4(distcolor,0) ;




                // sample the texture
                //fixed4 col = tex2D(_MainTex, o.uv);
                // apply fog
               // UNITY_APPLY_FOG(o.fogCoord, col);

            /*   
                    THIS CODE 

               //range of the normal lies between -1 to 1
               // by doing this calculation we are make the range between 0 to 1
               float4 normal =o.normal *0.5+0.5;
               return float4(normal);s

            */

            /* 
                THIS CODE IS USE TO UNDERSTAND THE LIGHT FALLING ON THE OBJECT

                float3 lightDir = normalize (float3(1,1,1));
                float3 lightcolor = float3(0.9,0.8,0.76);
                
                //This will calculate the light vetor and the normal vetor of uv co-oridante
                // this will show the calculate the light exposer on the object
                float lightfallon = dot(lightDir,o.normal);

               //the values her are return on the base of the RGB channel (ie Red,green,blue,Alpa)
                float3 diffuselight = lightcolor*lightfallon;
                return float4(diffuselight,0);

            */ 
                
                //float3 lightDir = normalize (float3(1,1,1));
            /*
            
                //Here "saturate" function clamping the range -1 to 1 into 0 to 1
                float lightfallon = saturate(dot(lightDir,o.normal));
            */
                //if we want only to clamp bottom side(lower range) of the range than we can use max
                
                float2 uv = o.uv0;
                
                
                //float t = Inverlerp(0.25,0.75,uv.y);
                float2 t = uv;
                
                //float t = Poaterize(8,uv.y);
                //float3 t = Mylerp(ColorA,ColorB,Poaterize(8,uv.y));
                
               
            
                // smoothstepis similar to the inverlerp but it has a easein easeout funtion and grap value are little different from the inverlerp


                //return t;
                

                float3 blended = lerp(ColorA,ColorB,t.y);
                if(t.x<0.025 || t.y <0.025 || t.x > 0.975 || t.y > 0.975)
                {
                    blended = float3(0,0,0);
                }
               // float inverselerpvalue = Inverlerp(ColorA,ColorB,blended);
              //  return(inverselerpvalue);
                //return float4(blended,0);
               
                
                

                // LIGHTING
                float4 lightDir = _WorldSpaceLightPos0;

                
                //     //Code there will we a problem will using the interpolated values of the o.norwal
                //     //The value of o.normal is interpolated value so the lenght of this vector might br less then 1
                //     //There will a problem ocurre due this value
                //    //to Check that uncommite this code
                    // float lightfallon = max(0,dot(lightDir,o.normal)); 
                

                //Resolving the problem of interpolated value of o.normal
                //We will normailze the o.normal value to make its lenght 1
                float3 normal = normalize(o.normal);
                float lightfallon = max(0,dot(lightDir,normal));
                lightfallon = step(0.05,lightfallon); 

              
                
                float3 lightcolor = _LightColor0.rgb;
                float3 directdiffuselight = lightcolor*lightfallon;

                //Ambient Light
                float3 ambientlight = float3(0.1,0.1,0.1);
                float3 diffuselight = ambientlight +directdiffuselight;

                //Direct Secpular light
                float3 campos = _WorldSpaceCameraPos    ;
                float3 fragtocam = campos - o.worldpos;
                float3 viewdir = normalize(fragtocam);

                
                //PHONG ALGO FOR SPECTULAR LIGHT-=-=-=--=-0=-=-==-=-=-=-=-=-=>
                // in "reflect" funtion the 1 parameter is input.viewDir and the second parameter is normal
                
                
                // //CODE
                // //problem occure due to interpolated value of o.normal
                 //float3 viewreflected = reflect(-viewdir , o.normal);
                   
            
                // Use normalize vale of o.normal
                    float3 viewreflected = reflect(-viewdir , normal);
                float3 Specularlightoff = max(0,dot(viewreflected,lightDir));
                
                
                //Modify gloss+
                Specularlightoff = pow(Specularlightoff,_Gloss);
                Specularlightoff = Poaterize(8,Specularlightoff);
                float3 directSpecular = Specularlightoff * lightcolor;
                   // return float4(Specularlightoff.xxx+ambientlight,0);

                

                
                // Other way of doing the reflection
               // float3 relectedray = fragtocam - o.normal*2*(dot(fragtocam,o.normal));

               // return float4(relectedray,0);       

              //CODE

                //Compsite 
                float3 fanilSurfacecolor = diffuselight * _Color.rgb + directSpecular; 
                return float4(fanilSurfacecolor,0);
            }
            ENDCG
        }
    }
}

