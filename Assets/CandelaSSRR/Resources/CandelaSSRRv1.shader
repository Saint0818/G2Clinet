Shader "Hidden/CandelaSSRRv1" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }
	Pass {
				Program "vp" {
// Vertex combos: 1
//   d3d9 - ALU: 5 to 5
//   d3d11 - ALU: 4 to 4, TEX: 0 to 0, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
"!!GLSL
#ifdef VERTEX
varying vec2 xlv_TEXCOORD0;

void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
varying vec2 xlv_TEXCOORD0;
uniform sampler2D _ColorTextureCustom;
uniform float _renderCustomColorMap;
uniform float _FlipReflectionsMSAA;
uniform float _SSRRcomposeMode;
uniform sampler2D _CameraNormalsTexture;
uniform mat4 _ViewMatrix;
uniform mat4 _ProjectionInv;
uniform mat4 _ProjMatrix;
uniform float _bias;
uniform float _stepGlobalScale;
uniform float _maxStep;
uniform float _maxFineStep;
uniform float _maxDepthCull;
uniform float _fadePower;
uniform sampler2D _MainTex;
uniform sampler2D _depthTexCustom;
uniform vec4 _ZBufferParams;
uniform vec4 _ScreenParams;
void main ()
{
  vec3 sspref3df_1;
  vec4 resfinaelxe_2;
  vec4 frefcol2d_3;
  vec4 tmpvar_4;
  tmpvar_4 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  if ((tmpvar_4.w == 0.0)) {
    frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
  } else {
    vec4 tmpvar_5;
    tmpvar_5 = texture2DLod (_depthTexCustom, xlv_TEXCOORD0, 0.0);
    float tmpvar_6;
    tmpvar_6 = tmpvar_5.x;
    float tmpvar_7;
    tmpvar_7 = (1.0/(((_ZBufferParams.x * tmpvar_5.x) + _ZBufferParams.y)));
    if ((tmpvar_7 > _maxDepthCull)) {
      frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
      vec4 pacolac2s_8;
      vec4 orgtpm4x_9;
      int s_10;
      vec4 decrect36s_11;
      int i33tyux_12;
      bool boo32df_13;
      vec4 hheropg_14;
      float ql30fg_15;
      int mx29iujh_16;
      vec3 sam27pio_17;
      vec3 v26o9ij_18;
      vec3 j23kqa_19;
      vec4 ght43s_20;
      vec4 nmgghg16y_21;
      vec3 oy15df_22;
      vec4 v12fefsk_23;
      int tmpvar_24;
      tmpvar_24 = int(_maxStep);
      v12fefsk_23.w = 1.0;
      v12fefsk_23.xy = ((xlv_TEXCOORD0 * 2.0) - 1.0);
      v12fefsk_23.z = tmpvar_6;
      vec4 tmpvar_25;
      tmpvar_25 = (_ProjectionInv * v12fefsk_23);
      vec4 tmpvar_26;
      tmpvar_26 = (tmpvar_25 / tmpvar_25.w);
      oy15df_22.xy = v12fefsk_23.xy;
      oy15df_22.z = tmpvar_6;
      nmgghg16y_21.w = 0.0;
      nmgghg16y_21.xyz = ((texture2DLod (_CameraNormalsTexture, xlv_TEXCOORD0, 0.0).xyz * 2.0) - 1.0);
      vec3 tmpvar_27;
      tmpvar_27 = normalize(tmpvar_26.xyz);
      vec3 tmpvar_28;
      tmpvar_28 = normalize((_ViewMatrix * nmgghg16y_21).xyz);
      vec3 tmpvar_29;
      tmpvar_29 = normalize((tmpvar_27 - (2.0 * (dot (tmpvar_28, tmpvar_27) * tmpvar_28))));
      ght43s_20.w = 1.0;
      ght43s_20.xyz = (tmpvar_26.xyz + tmpvar_29);
      vec4 tmpvar_30;
      tmpvar_30 = (_ProjMatrix * ght43s_20);
      vec3 tmpvar_31;
      tmpvar_31 = normalize(((tmpvar_30.xyz / tmpvar_30.w) - oy15df_22));
      sspref3df_1.z = tmpvar_31.z;
      sspref3df_1.xy = (tmpvar_31.xy * 0.5);
      j23kqa_19.xy = xlv_TEXCOORD0;
      j23kqa_19.z = tmpvar_6;
      float tmpvar_32;
      tmpvar_32 = (2.0 / _ScreenParams.x);
      float tmpvar_33;
      tmpvar_33 = sqrt(dot (sspref3df_1.xy, sspref3df_1.xy));
      vec3 tmpvar_34;
      tmpvar_34 = (sspref3df_1 * ((tmpvar_32 * _stepGlobalScale) / tmpvar_33));
      v26o9ij_18 = tmpvar_34;
      mx29iujh_16 = int(_maxStep);
      ql30fg_15 = 0.0;
      boo32df_13 = bool(0);
      sam27pio_17 = (j23kqa_19 + tmpvar_34);
      i33tyux_12 = 0;
      s_10 = 0;
      for (int s_10 = 0; s_10 < 99; ) {
        if ((i33tyux_12 >= mx29iujh_16)) {
          break;
        };
        float tmpvar_35;
        tmpvar_35 = (1.0/(((_ZBufferParams.x * texture2DLod (_depthTexCustom, sam27pio_17.xy, 0.0).x) + _ZBufferParams.y)));
        float tmpvar_36;
        tmpvar_36 = (1.0/(((_ZBufferParams.x * sam27pio_17.z) + _ZBufferParams.y)));
        if ((tmpvar_35 < (tmpvar_36 - 1e-06))) {
          decrect36s_11.w = 1.0;
          decrect36s_11.xyz = sam27pio_17;
          hheropg_14 = decrect36s_11;
          boo32df_13 = bool(1);
          break;
        };
        sam27pio_17 = (sam27pio_17 + v26o9ij_18);
        ql30fg_15 = (ql30fg_15 + 1.0);
        i33tyux_12 = (i33tyux_12 + 1);
        s_10 = (s_10 + 1);
      };
      if ((boo32df_13 == bool(0))) {
        vec4 tpv37xs_37;
        tpv37xs_37.w = 0.0;
        tpv37xs_37.xyz = sam27pio_17;
        hheropg_14 = tpv37xs_37;
        boo32df_13 = bool(1);
      };
      resfinaelxe_2 = hheropg_14;
      float tmpvar_38;
      tmpvar_38 = abs((hheropg_14.x - 0.5));
      orgtpm4x_9 = tmpvar_4;
      if ((_FlipReflectionsMSAA > 0.0)) {
        vec2 tmpouv_39;
        tmpouv_39.x = xlv_TEXCOORD0.x;
        tmpouv_39.y = (1.0 - xlv_TEXCOORD0.y);
        orgtpm4x_9 = texture2DLod (_MainTex, tmpouv_39, 0.0);
      };
      pacolac2s_8 = vec4(0.0, 0.0, 0.0, 0.0);
      if ((_SSRRcomposeMode > 0.0)) {
        vec4 tmpvar_40;
        tmpvar_40.w = 0.0;
        tmpvar_40.xyz = orgtpm4x_9.xyz;
        pacolac2s_8 = tmpvar_40;
      };
      if ((tmpvar_38 > 0.5)) {
        frefcol2d_3 = pacolac2s_8;
      } else {
        float tmpvar_41;
        tmpvar_41 = abs((hheropg_14.y - 0.5));
        if ((tmpvar_41 > 0.5)) {
          frefcol2d_3 = pacolac2s_8;
        } else {
          if (((1.0/(((_ZBufferParams.x * hheropg_14.z) + _ZBufferParams.y))) > _maxDepthCull)) {
            frefcol2d_3 = pacolac2s_8;
          } else {
            if ((hheropg_14.z < 0.1)) {
              frefcol2d_3 = pacolac2s_8;
            } else {
              if ((hheropg_14.w == 1.0)) {
                int j_42;
                vec4 iotared_43;
                vec3 opl50op_44;
                int i49opght_45;
                bool oopplx_46;
                vec4 ghfjghtbbv_47;
                int mxc45ui_48;
                vec3 freg44r_49;
                vec3 ps43testy_50;
                vec3 tmpvar_51;
                tmpvar_51 = (hheropg_14.xyz - tmpvar_34);
                vec3 tmpvar_52;
                tmpvar_52 = (sspref3df_1 * (tmpvar_32 / tmpvar_33));
                freg44r_49 = tmpvar_52;
                mxc45ui_48 = int(_maxFineStep);
                oopplx_46 = bool(0);
                opl50op_44 = tmpvar_51;
                ps43testy_50 = (tmpvar_51 + tmpvar_52);
                i49opght_45 = 0;
                j_42 = 0;
                for (int j_42 = 0; j_42 < 20; ) {
                  if ((i49opght_45 >= mxc45ui_48)) {
                    break;
                  };
                  float tmpvar_53;
                  tmpvar_53 = (1.0/(((_ZBufferParams.x * texture2DLod (_depthTexCustom, ps43testy_50.xy, 0.0).x) + _ZBufferParams.y)));
                  float tmpvar_54;
                  tmpvar_54 = (1.0/(((_ZBufferParams.x * ps43testy_50.z) + _ZBufferParams.y)));
                  if ((tmpvar_53 < tmpvar_54)) {
                    if (((tmpvar_54 - tmpvar_53) < _bias)) {
                      iotared_43.w = 1.0;
                      iotared_43.xyz = ps43testy_50;
                      ghfjghtbbv_47 = iotared_43;
                      oopplx_46 = bool(1);
                      break;
                    };
                    vec3 tmpvar_55;
                    tmpvar_55 = (freg44r_49 * 0.5);
                    freg44r_49 = tmpvar_55;
                    ps43testy_50 = (opl50op_44 + tmpvar_55);
                  } else {
                    opl50op_44 = ps43testy_50;
                    ps43testy_50 = (ps43testy_50 + freg44r_49);
                  };
                  i49opght_45 = (i49opght_45 + 1);
                  j_42 = (j_42 + 1);
                };
                if ((oopplx_46 == bool(0))) {
                  vec4 vsap55f_56;
                  vsap55f_56.w = 0.0;
                  vsap55f_56.xyz = ps43testy_50;
                  ghfjghtbbv_47 = vsap55f_56;
                  oopplx_46 = bool(1);
                };
                resfinaelxe_2 = ghfjghtbbv_47;
              };
              if ((resfinaelxe_2.w < 0.01)) {
                frefcol2d_3 = pacolac2s_8;
              } else {
                vec2 tmpres3_57;
                vec4 ui57tefrt_58;
                tmpres3_57 = resfinaelxe_2.xy;
                if ((_FlipReflectionsMSAA > 0.0)) {
                  resfinaelxe_2.y = (1.0 - resfinaelxe_2.y);
                  tmpres3_57.x = resfinaelxe_2.x;
                  tmpres3_57.y = (1.0 - resfinaelxe_2.y);
                };
                vec4 tmpvar_59;
                tmpvar_59 = texture2DLod (_depthTexCustom, tmpres3_57, 0.0);
                if ((tmpvar_59.y > 0.1)) {
                  if ((_renderCustomColorMap < 0.5)) {
                    ui57tefrt_58.xyz = texture2DLod (_MainTex, resfinaelxe_2.xy, 0.0).xyz;
                  } else {
                    ui57tefrt_58.xyz = texture2DLod (_ColorTextureCustom, resfinaelxe_2.xy, 0.0).xyz;
                  };
                  ui57tefrt_58.w = (((resfinaelxe_2.w * (1.0 - (tmpvar_7 / _maxDepthCull))) * (1.0 - pow ((ql30fg_15 / float(tmpvar_24)), _fadePower))) * pow (clamp (((dot (normalize(tmpvar_29), normalize(tmpvar_26).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                  frefcol2d_3 = ui57tefrt_58;
                } else {
                  frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
                };
              };
            };
          };
        };
      };
    };
  };
  gl_FragData[0] = frefcol2d_3;
}


#endif
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"vs_3_0
; 5 ALU
dcl_position o0
dcl_texcoord0 o1
dcl_position0 v0
dcl_texcoord0 v1
mov o1.xy, v1
dp4 o0.w, v0, c3
dp4 o0.z, v0, c2
dp4 o0.y, v0, c1
dp4 o0.x, v0, c0
"
}

SubProgram "d3d11 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
BindCB "UnityPerDraw" 0
// 6 instructions, 1 temp regs, 0 temp arrays:
// ALU 4 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec2 xlv_TEXCOORD0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shader_texture_lod : enable
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _ColorTextureCustom;
uniform highp float _renderCustomColorMap;
uniform highp float _FlipReflectionsMSAA;
uniform highp float _SSRRcomposeMode;
uniform sampler2D _CameraNormalsTexture;
uniform highp mat4 _ViewMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ProjMatrix;
uniform highp float _bias;
uniform highp float _stepGlobalScale;
uniform highp float _maxStep;
uniform highp float _maxFineStep;
uniform highp float _maxDepthCull;
uniform highp float _fadePower;
uniform sampler2D _MainTex;
uniform sampler2D _depthTexCustom;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _ScreenParams;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 osdfej3_2;
  highp vec3 sspref3df_3;
  highp vec4 resfinaelxe_4;
  highp vec4 frefcol2d_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2DLodEXT (_MainTex, xlv_TEXCOORD0, 0.0);
  osdfej3_2 = tmpvar_6;
  if ((osdfej3_2.w == 0.0)) {
    frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
  } else {
    highp float feoimdf_7;
    lowp float tmpvar_8;
    tmpvar_8 = texture2DLodEXT (_depthTexCustom, xlv_TEXCOORD0, 0.0).x;
    feoimdf_7 = tmpvar_8;
    highp float tmpvar_9;
    tmpvar_9 = (1.0/(((_ZBufferParams.x * feoimdf_7) + _ZBufferParams.y)));
    if ((tmpvar_9 > _maxDepthCull)) {
      frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
      highp vec4 pacolac2s_10;
      highp vec4 orgtpm4x_11;
      int s_12;
      highp vec4 decrect36s_13;
      int i33tyux_14;
      bool boo32df_15;
      highp vec4 hheropg_16;
      highp float ql30fg_17;
      int mx29iujh_18;
      highp vec3 sam27pio_19;
      highp vec3 v26o9ij_20;
      highp vec3 j23kqa_21;
      highp vec4 ght43s_22;
      highp vec4 nmgghg16y_23;
      highp vec3 oy15df_24;
      highp vec4 v12fefsk_25;
      int tmpvar_26;
      tmpvar_26 = int(_maxStep);
      v12fefsk_25.w = 1.0;
      v12fefsk_25.xy = ((xlv_TEXCOORD0 * 2.0) - 1.0);
      v12fefsk_25.z = feoimdf_7;
      highp vec4 tmpvar_27;
      tmpvar_27 = (_ProjectionInv * v12fefsk_25);
      highp vec4 tmpvar_28;
      tmpvar_28 = (tmpvar_27 / tmpvar_27.w);
      oy15df_24.xy = v12fefsk_25.xy;
      oy15df_24.z = feoimdf_7;
      nmgghg16y_23.w = 0.0;
      lowp vec3 tmpvar_29;
      tmpvar_29 = ((texture2DLodEXT (_CameraNormalsTexture, xlv_TEXCOORD0, 0.0).xyz * 2.0) - 1.0);
      nmgghg16y_23.xyz = tmpvar_29;
      highp vec3 tmpvar_30;
      tmpvar_30 = normalize(tmpvar_28.xyz);
      highp vec3 tmpvar_31;
      tmpvar_31 = normalize((_ViewMatrix * nmgghg16y_23).xyz);
      highp vec3 tmpvar_32;
      tmpvar_32 = normalize((tmpvar_30 - (2.0 * (dot (tmpvar_31, tmpvar_30) * tmpvar_31))));
      ght43s_22.w = 1.0;
      ght43s_22.xyz = (tmpvar_28.xyz + tmpvar_32);
      highp vec4 tmpvar_33;
      tmpvar_33 = (_ProjMatrix * ght43s_22);
      highp vec3 tmpvar_34;
      tmpvar_34 = normalize(((tmpvar_33.xyz / tmpvar_33.w) - oy15df_24));
      sspref3df_3.z = tmpvar_34.z;
      sspref3df_3.xy = (tmpvar_34.xy * 0.5);
      j23kqa_21.xy = xlv_TEXCOORD0;
      j23kqa_21.z = feoimdf_7;
      highp float tmpvar_35;
      tmpvar_35 = (2.0 / _ScreenParams.x);
      highp float tmpvar_36;
      tmpvar_36 = sqrt(dot (sspref3df_3.xy, sspref3df_3.xy));
      highp vec3 tmpvar_37;
      tmpvar_37 = (sspref3df_3 * ((tmpvar_35 * _stepGlobalScale) / tmpvar_36));
      v26o9ij_20 = tmpvar_37;
      mx29iujh_18 = int(_maxStep);
      ql30fg_17 = 0.0;
      boo32df_15 = bool(0);
      sam27pio_19 = (j23kqa_21 + tmpvar_37);
      i33tyux_14 = 0;
      s_12 = 0;
      for (int s_12 = 0; s_12 < 99; ) {
        if ((i33tyux_14 >= mx29iujh_18)) {
          break;
        };
        lowp vec4 tmpvar_38;
        tmpvar_38 = texture2DLodEXT (_depthTexCustom, sam27pio_19.xy, 0.0);
        highp float tmpvar_39;
        tmpvar_39 = (1.0/(((_ZBufferParams.x * tmpvar_38.x) + _ZBufferParams.y)));
        highp float tmpvar_40;
        tmpvar_40 = (1.0/(((_ZBufferParams.x * sam27pio_19.z) + _ZBufferParams.y)));
        if ((tmpvar_39 < (tmpvar_40 - 1e-06))) {
          decrect36s_13.w = 1.0;
          decrect36s_13.xyz = sam27pio_19;
          hheropg_16 = decrect36s_13;
          boo32df_15 = bool(1);
          break;
        };
        sam27pio_19 = (sam27pio_19 + v26o9ij_20);
        ql30fg_17 = (ql30fg_17 + 1.0);
        i33tyux_14 = (i33tyux_14 + 1);
        s_12 = (s_12 + 1);
      };
      if ((boo32df_15 == bool(0))) {
        highp vec4 tpv37xs_41;
        tpv37xs_41.w = 0.0;
        tpv37xs_41.xyz = sam27pio_19;
        hheropg_16 = tpv37xs_41;
        boo32df_15 = bool(1);
      };
      resfinaelxe_4 = hheropg_16;
      highp float tmpvar_42;
      tmpvar_42 = abs((hheropg_16.x - 0.5));
      orgtpm4x_11 = osdfej3_2;
      if ((_FlipReflectionsMSAA > 0.0)) {
        highp vec2 tmpouv_43;
        tmpouv_43.x = xlv_TEXCOORD0.x;
        tmpouv_43.y = (1.0 - xlv_TEXCOORD0.y);
        lowp vec4 tmpvar_44;
        tmpvar_44 = texture2DLodEXT (_MainTex, tmpouv_43, 0.0);
        orgtpm4x_11 = tmpvar_44;
      };
      pacolac2s_10 = vec4(0.0, 0.0, 0.0, 0.0);
      if ((_SSRRcomposeMode > 0.0)) {
        highp vec4 tmpvar_45;
        tmpvar_45.w = 0.0;
        tmpvar_45.xyz = orgtpm4x_11.xyz;
        pacolac2s_10 = tmpvar_45;
      };
      if ((tmpvar_42 > 0.5)) {
        frefcol2d_5 = pacolac2s_10;
      } else {
        highp float tmpvar_46;
        tmpvar_46 = abs((hheropg_16.y - 0.5));
        if ((tmpvar_46 > 0.5)) {
          frefcol2d_5 = pacolac2s_10;
        } else {
          if (((1.0/(((_ZBufferParams.x * hheropg_16.z) + _ZBufferParams.y))) > _maxDepthCull)) {
            frefcol2d_5 = pacolac2s_10;
          } else {
            if ((hheropg_16.z < 0.1)) {
              frefcol2d_5 = pacolac2s_10;
            } else {
              if ((hheropg_16.w == 1.0)) {
                int j_47;
                highp vec4 iotared_48;
                highp vec3 opl50op_49;
                int i49opght_50;
                bool oopplx_51;
                highp vec4 ghfjghtbbv_52;
                int mxc45ui_53;
                highp vec3 freg44r_54;
                highp vec3 ps43testy_55;
                highp vec3 tmpvar_56;
                tmpvar_56 = (hheropg_16.xyz - tmpvar_37);
                highp vec3 tmpvar_57;
                tmpvar_57 = (sspref3df_3 * (tmpvar_35 / tmpvar_36));
                freg44r_54 = tmpvar_57;
                mxc45ui_53 = int(_maxFineStep);
                oopplx_51 = bool(0);
                opl50op_49 = tmpvar_56;
                ps43testy_55 = (tmpvar_56 + tmpvar_57);
                i49opght_50 = 0;
                j_47 = 0;
                for (int j_47 = 0; j_47 < 20; ) {
                  if ((i49opght_50 >= mxc45ui_53)) {
                    break;
                  };
                  lowp vec4 tmpvar_58;
                  tmpvar_58 = texture2DLodEXT (_depthTexCustom, ps43testy_55.xy, 0.0);
                  highp float tmpvar_59;
                  tmpvar_59 = (1.0/(((_ZBufferParams.x * tmpvar_58.x) + _ZBufferParams.y)));
                  highp float tmpvar_60;
                  tmpvar_60 = (1.0/(((_ZBufferParams.x * ps43testy_55.z) + _ZBufferParams.y)));
                  if ((tmpvar_59 < tmpvar_60)) {
                    if (((tmpvar_60 - tmpvar_59) < _bias)) {
                      iotared_48.w = 1.0;
                      iotared_48.xyz = ps43testy_55;
                      ghfjghtbbv_52 = iotared_48;
                      oopplx_51 = bool(1);
                      break;
                    };
                    highp vec3 tmpvar_61;
                    tmpvar_61 = (freg44r_54 * 0.5);
                    freg44r_54 = tmpvar_61;
                    ps43testy_55 = (opl50op_49 + tmpvar_61);
                  } else {
                    opl50op_49 = ps43testy_55;
                    ps43testy_55 = (ps43testy_55 + freg44r_54);
                  };
                  i49opght_50 = (i49opght_50 + 1);
                  j_47 = (j_47 + 1);
                };
                if ((oopplx_51 == bool(0))) {
                  highp vec4 vsap55f_62;
                  vsap55f_62.w = 0.0;
                  vsap55f_62.xyz = ps43testy_55;
                  ghfjghtbbv_52 = vsap55f_62;
                  oopplx_51 = bool(1);
                };
                resfinaelxe_4 = ghfjghtbbv_52;
              };
              if ((resfinaelxe_4.w < 0.01)) {
                frefcol2d_5 = pacolac2s_10;
              } else {
                highp vec2 tmpres3_63;
                highp vec4 ui57tefrt_64;
                tmpres3_63 = resfinaelxe_4.xy;
                if ((_FlipReflectionsMSAA > 0.0)) {
                  resfinaelxe_4.y = (1.0 - resfinaelxe_4.y);
                  tmpres3_63.x = resfinaelxe_4.x;
                  tmpres3_63.y = (1.0 - resfinaelxe_4.y);
                };
                lowp vec4 tmpvar_65;
                tmpvar_65 = texture2DLodEXT (_depthTexCustom, tmpres3_63, 0.0);
                if ((tmpvar_65.y > 0.1)) {
                  if ((_renderCustomColorMap < 0.5)) {
                    lowp vec3 tmpvar_66;
                    tmpvar_66 = texture2DLodEXT (_MainTex, resfinaelxe_4.xy, 0.0).xyz;
                    ui57tefrt_64.xyz = tmpvar_66;
                  } else {
                    lowp vec3 tmpvar_67;
                    tmpvar_67 = texture2DLodEXT (_ColorTextureCustom, resfinaelxe_4.xy, 0.0).xyz;
                    ui57tefrt_64.xyz = tmpvar_67;
                  };
                  ui57tefrt_64.w = (((resfinaelxe_4.w * (1.0 - (tmpvar_9 / _maxDepthCull))) * (1.0 - pow ((ql30fg_17 / float(tmpvar_26)), _fadePower))) * pow (clamp (((dot (normalize(tmpvar_32), normalize(tmpvar_28).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                  frefcol2d_5 = ui57tefrt_64;
                } else {
                  frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
                };
              };
            };
          };
        };
      };
    };
  };
  tmpvar_1 = frefcol2d_5;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec2 xlv_TEXCOORD0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shader_texture_lod : enable
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _ColorTextureCustom;
uniform highp float _renderCustomColorMap;
uniform highp float _FlipReflectionsMSAA;
uniform highp float _SSRRcomposeMode;
uniform sampler2D _CameraNormalsTexture;
uniform highp mat4 _ViewMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ProjMatrix;
uniform highp float _bias;
uniform highp float _stepGlobalScale;
uniform highp float _maxStep;
uniform highp float _maxFineStep;
uniform highp float _maxDepthCull;
uniform highp float _fadePower;
uniform sampler2D _MainTex;
uniform sampler2D _depthTexCustom;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _ScreenParams;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 osdfej3_2;
  highp vec3 sspref3df_3;
  highp vec4 resfinaelxe_4;
  highp vec4 frefcol2d_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2DLodEXT (_MainTex, xlv_TEXCOORD0, 0.0);
  osdfej3_2 = tmpvar_6;
  if ((osdfej3_2.w == 0.0)) {
    frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
  } else {
    highp float feoimdf_7;
    lowp float tmpvar_8;
    tmpvar_8 = texture2DLodEXT (_depthTexCustom, xlv_TEXCOORD0, 0.0).x;
    feoimdf_7 = tmpvar_8;
    highp float tmpvar_9;
    tmpvar_9 = (1.0/(((_ZBufferParams.x * feoimdf_7) + _ZBufferParams.y)));
    if ((tmpvar_9 > _maxDepthCull)) {
      frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
      highp vec4 pacolac2s_10;
      highp vec4 orgtpm4x_11;
      int s_12;
      highp vec4 decrect36s_13;
      int i33tyux_14;
      bool boo32df_15;
      highp vec4 hheropg_16;
      highp float ql30fg_17;
      int mx29iujh_18;
      highp vec3 sam27pio_19;
      highp vec3 v26o9ij_20;
      highp vec3 j23kqa_21;
      highp vec4 ght43s_22;
      highp vec4 nmgghg16y_23;
      highp vec3 oy15df_24;
      highp vec4 v12fefsk_25;
      int tmpvar_26;
      tmpvar_26 = int(_maxStep);
      v12fefsk_25.w = 1.0;
      v12fefsk_25.xy = ((xlv_TEXCOORD0 * 2.0) - 1.0);
      v12fefsk_25.z = feoimdf_7;
      highp vec4 tmpvar_27;
      tmpvar_27 = (_ProjectionInv * v12fefsk_25);
      highp vec4 tmpvar_28;
      tmpvar_28 = (tmpvar_27 / tmpvar_27.w);
      oy15df_24.xy = v12fefsk_25.xy;
      oy15df_24.z = feoimdf_7;
      nmgghg16y_23.w = 0.0;
      lowp vec3 tmpvar_29;
      tmpvar_29 = ((texture2DLodEXT (_CameraNormalsTexture, xlv_TEXCOORD0, 0.0).xyz * 2.0) - 1.0);
      nmgghg16y_23.xyz = tmpvar_29;
      highp vec3 tmpvar_30;
      tmpvar_30 = normalize(tmpvar_28.xyz);
      highp vec3 tmpvar_31;
      tmpvar_31 = normalize((_ViewMatrix * nmgghg16y_23).xyz);
      highp vec3 tmpvar_32;
      tmpvar_32 = normalize((tmpvar_30 - (2.0 * (dot (tmpvar_31, tmpvar_30) * tmpvar_31))));
      ght43s_22.w = 1.0;
      ght43s_22.xyz = (tmpvar_28.xyz + tmpvar_32);
      highp vec4 tmpvar_33;
      tmpvar_33 = (_ProjMatrix * ght43s_22);
      highp vec3 tmpvar_34;
      tmpvar_34 = normalize(((tmpvar_33.xyz / tmpvar_33.w) - oy15df_24));
      sspref3df_3.z = tmpvar_34.z;
      sspref3df_3.xy = (tmpvar_34.xy * 0.5);
      j23kqa_21.xy = xlv_TEXCOORD0;
      j23kqa_21.z = feoimdf_7;
      highp float tmpvar_35;
      tmpvar_35 = (2.0 / _ScreenParams.x);
      highp float tmpvar_36;
      tmpvar_36 = sqrt(dot (sspref3df_3.xy, sspref3df_3.xy));
      highp vec3 tmpvar_37;
      tmpvar_37 = (sspref3df_3 * ((tmpvar_35 * _stepGlobalScale) / tmpvar_36));
      v26o9ij_20 = tmpvar_37;
      mx29iujh_18 = int(_maxStep);
      ql30fg_17 = 0.0;
      boo32df_15 = bool(0);
      sam27pio_19 = (j23kqa_21 + tmpvar_37);
      i33tyux_14 = 0;
      s_12 = 0;
      for (int s_12 = 0; s_12 < 99; ) {
        if ((i33tyux_14 >= mx29iujh_18)) {
          break;
        };
        lowp vec4 tmpvar_38;
        tmpvar_38 = texture2DLodEXT (_depthTexCustom, sam27pio_19.xy, 0.0);
        highp float tmpvar_39;
        tmpvar_39 = (1.0/(((_ZBufferParams.x * tmpvar_38.x) + _ZBufferParams.y)));
        highp float tmpvar_40;
        tmpvar_40 = (1.0/(((_ZBufferParams.x * sam27pio_19.z) + _ZBufferParams.y)));
        if ((tmpvar_39 < (tmpvar_40 - 1e-06))) {
          decrect36s_13.w = 1.0;
          decrect36s_13.xyz = sam27pio_19;
          hheropg_16 = decrect36s_13;
          boo32df_15 = bool(1);
          break;
        };
        sam27pio_19 = (sam27pio_19 + v26o9ij_20);
        ql30fg_17 = (ql30fg_17 + 1.0);
        i33tyux_14 = (i33tyux_14 + 1);
        s_12 = (s_12 + 1);
      };
      if ((boo32df_15 == bool(0))) {
        highp vec4 tpv37xs_41;
        tpv37xs_41.w = 0.0;
        tpv37xs_41.xyz = sam27pio_19;
        hheropg_16 = tpv37xs_41;
        boo32df_15 = bool(1);
      };
      resfinaelxe_4 = hheropg_16;
      highp float tmpvar_42;
      tmpvar_42 = abs((hheropg_16.x - 0.5));
      orgtpm4x_11 = osdfej3_2;
      if ((_FlipReflectionsMSAA > 0.0)) {
        highp vec2 tmpouv_43;
        tmpouv_43.x = xlv_TEXCOORD0.x;
        tmpouv_43.y = (1.0 - xlv_TEXCOORD0.y);
        lowp vec4 tmpvar_44;
        tmpvar_44 = texture2DLodEXT (_MainTex, tmpouv_43, 0.0);
        orgtpm4x_11 = tmpvar_44;
      };
      pacolac2s_10 = vec4(0.0, 0.0, 0.0, 0.0);
      if ((_SSRRcomposeMode > 0.0)) {
        highp vec4 tmpvar_45;
        tmpvar_45.w = 0.0;
        tmpvar_45.xyz = orgtpm4x_11.xyz;
        pacolac2s_10 = tmpvar_45;
      };
      if ((tmpvar_42 > 0.5)) {
        frefcol2d_5 = pacolac2s_10;
      } else {
        highp float tmpvar_46;
        tmpvar_46 = abs((hheropg_16.y - 0.5));
        if ((tmpvar_46 > 0.5)) {
          frefcol2d_5 = pacolac2s_10;
        } else {
          if (((1.0/(((_ZBufferParams.x * hheropg_16.z) + _ZBufferParams.y))) > _maxDepthCull)) {
            frefcol2d_5 = pacolac2s_10;
          } else {
            if ((hheropg_16.z < 0.1)) {
              frefcol2d_5 = pacolac2s_10;
            } else {
              if ((hheropg_16.w == 1.0)) {
                int j_47;
                highp vec4 iotared_48;
                highp vec3 opl50op_49;
                int i49opght_50;
                bool oopplx_51;
                highp vec4 ghfjghtbbv_52;
                int mxc45ui_53;
                highp vec3 freg44r_54;
                highp vec3 ps43testy_55;
                highp vec3 tmpvar_56;
                tmpvar_56 = (hheropg_16.xyz - tmpvar_37);
                highp vec3 tmpvar_57;
                tmpvar_57 = (sspref3df_3 * (tmpvar_35 / tmpvar_36));
                freg44r_54 = tmpvar_57;
                mxc45ui_53 = int(_maxFineStep);
                oopplx_51 = bool(0);
                opl50op_49 = tmpvar_56;
                ps43testy_55 = (tmpvar_56 + tmpvar_57);
                i49opght_50 = 0;
                j_47 = 0;
                for (int j_47 = 0; j_47 < 20; ) {
                  if ((i49opght_50 >= mxc45ui_53)) {
                    break;
                  };
                  lowp vec4 tmpvar_58;
                  tmpvar_58 = texture2DLodEXT (_depthTexCustom, ps43testy_55.xy, 0.0);
                  highp float tmpvar_59;
                  tmpvar_59 = (1.0/(((_ZBufferParams.x * tmpvar_58.x) + _ZBufferParams.y)));
                  highp float tmpvar_60;
                  tmpvar_60 = (1.0/(((_ZBufferParams.x * ps43testy_55.z) + _ZBufferParams.y)));
                  if ((tmpvar_59 < tmpvar_60)) {
                    if (((tmpvar_60 - tmpvar_59) < _bias)) {
                      iotared_48.w = 1.0;
                      iotared_48.xyz = ps43testy_55;
                      ghfjghtbbv_52 = iotared_48;
                      oopplx_51 = bool(1);
                      break;
                    };
                    highp vec3 tmpvar_61;
                    tmpvar_61 = (freg44r_54 * 0.5);
                    freg44r_54 = tmpvar_61;
                    ps43testy_55 = (opl50op_49 + tmpvar_61);
                  } else {
                    opl50op_49 = ps43testy_55;
                    ps43testy_55 = (ps43testy_55 + freg44r_54);
                  };
                  i49opght_50 = (i49opght_50 + 1);
                  j_47 = (j_47 + 1);
                };
                if ((oopplx_51 == bool(0))) {
                  highp vec4 vsap55f_62;
                  vsap55f_62.w = 0.0;
                  vsap55f_62.xyz = ps43testy_55;
                  ghfjghtbbv_52 = vsap55f_62;
                  oopplx_51 = bool(1);
                };
                resfinaelxe_4 = ghfjghtbbv_52;
              };
              if ((resfinaelxe_4.w < 0.01)) {
                frefcol2d_5 = pacolac2s_10;
              } else {
                highp vec2 tmpres3_63;
                highp vec4 ui57tefrt_64;
                tmpres3_63 = resfinaelxe_4.xy;
                if ((_FlipReflectionsMSAA > 0.0)) {
                  resfinaelxe_4.y = (1.0 - resfinaelxe_4.y);
                  tmpres3_63.x = resfinaelxe_4.x;
                  tmpres3_63.y = (1.0 - resfinaelxe_4.y);
                };
                lowp vec4 tmpvar_65;
                tmpvar_65 = texture2DLodEXT (_depthTexCustom, tmpres3_63, 0.0);
                if ((tmpvar_65.y > 0.1)) {
                  if ((_renderCustomColorMap < 0.5)) {
                    lowp vec3 tmpvar_66;
                    tmpvar_66 = texture2DLodEXT (_MainTex, resfinaelxe_4.xy, 0.0).xyz;
                    ui57tefrt_64.xyz = tmpvar_66;
                  } else {
                    lowp vec3 tmpvar_67;
                    tmpvar_67 = texture2DLodEXT (_ColorTextureCustom, resfinaelxe_4.xy, 0.0).xyz;
                    ui57tefrt_64.xyz = tmpvar_67;
                  };
                  ui57tefrt_64.w = (((resfinaelxe_4.w * (1.0 - (tmpvar_9 / _maxDepthCull))) * (1.0 - pow ((ql30fg_17 / float(tmpvar_26)), _fadePower))) * pow (clamp (((dot (normalize(tmpvar_32), normalize(tmpvar_28).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                  frefcol2d_5 = ui57tefrt_64;
                } else {
                  frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
                };
              };
            };
          };
        };
      };
    };
  };
  tmpvar_1 = frefcol2d_5;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;

#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 333
struct v2f {
    highp vec4 pos;
    highp vec2 uv;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform sampler2D _depthTexCustom;
uniform sampler2D _MainTex;
uniform highp float _fadePower;
uniform highp float _maxDepthCull;
#line 319
uniform highp float _maxFineStep;
uniform highp float _maxStep;
uniform highp float _stepGlobalScale;
uniform highp float _bias;
#line 323
uniform highp mat4 _ProjMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ViewMatrix;
uniform highp vec4 _ProjInfo;
#line 327
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
uniform highp float _SSRRcomposeMode;
uniform highp float _FlipReflectionsMSAA;
#line 331
uniform highp float _renderCustomColorMap;
uniform sampler2D _ColorTextureCustom;
#line 339
#line 339
v2f vert( in appdata_img v ) {
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    #line 343
    o.uv = v.texcoord.xy;
    return o;
}
out highp vec2 xlv_TEXCOORD0;
void main() {
    v2f xl_retval;
    appdata_img xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];
vec4 xll_tex2Dlod(sampler2D s, vec4 coord) {
   return textureLod( s, coord.xy, coord.w);
}
#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 333
struct v2f {
    highp vec4 pos;
    highp vec2 uv;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform sampler2D _depthTexCustom;
uniform sampler2D _MainTex;
uniform highp float _fadePower;
uniform highp float _maxDepthCull;
#line 319
uniform highp float _maxFineStep;
uniform highp float _maxStep;
uniform highp float _stepGlobalScale;
uniform highp float _bias;
#line 323
uniform highp mat4 _ProjMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ViewMatrix;
uniform highp vec4 _ProjInfo;
#line 327
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
uniform highp float _SSRRcomposeMode;
uniform highp float _FlipReflectionsMSAA;
#line 331
uniform highp float _renderCustomColorMap;
uniform sampler2D _ColorTextureCustom;
#line 339
#line 346
mediump vec4 frag( in v2f i ) {
    #line 348
    highp vec4 frefcol2d;
    highp float len2ovaed;
    highp vec4 resfinaelxe;
    highp vec3 sspref3df;
    #line 352
    highp int maxcdfoief;
    highp vec4 osdfej3 = xll_tex2Dlod( _MainTex, vec4( i.uv, 0.0, 0.0));
    if ((osdfej3.w == 0.0)){
        #line 356
        frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
    }
    else{
        #line 360
        highp float feoimdf = xll_tex2Dlod( _depthTexCustom, vec4( i.uv, 0.0, 0.0)).x;
        highp float efopafeod2s = feoimdf;
        highp float v11dflke = (1.0 / ((_ZBufferParams.x * feoimdf) + _ZBufferParams.y));
        if ((v11dflke > _maxDepthCull)){
            #line 365
            frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
        }
        else{
            #line 369
            maxcdfoief = int(_maxStep);
            highp vec4 v12fefsk;
            v12fefsk.w = 1.0;
            v12fefsk.xy = ((i.uv * 2.0) - 1.0);
            #line 373
            v12fefsk.z = efopafeod2s;
            highp vec4 v13uujgh = (_ProjectionInv * v12fefsk);
            highp vec4 v14kkmng = (v13uujgh / v13uujgh.w);
            highp vec3 oy15df;
            #line 377
            oy15df.xy = v12fefsk.xy;
            oy15df.z = efopafeod2s;
            highp vec4 nmgghg16y;
            nmgghg16y.w = 0.0;
            #line 381
            nmgghg16y.xyz = ((xll_tex2Dlod( _CameraNormalsTexture, vec4( i.uv, 0.0, 0.0)).xyz * 2.0) - 1.0);
            highp vec3 uuyq32d = normalize(v14kkmng.xyz);
            highp vec3 f18iop = normalize((_ViewMatrix * nmgghg16y).xyz);
            highp vec3 v19vvdss = normalize((uuyq32d - (2.0 * (dot( f18iop, uuyq32d) * f18iop))));
            #line 385
            highp vec4 ght43s;
            ght43s.w = 1.0;
            ght43s.xyz = (v14kkmng.xyz + v19vvdss);
            highp vec4 retdfsqs = (_ProjMatrix * ght43s);
            #line 389
            highp vec3 t22ucvf = normalize(((retdfsqs.xyz / retdfsqs.w) - oy15df));
            sspref3df.z = t22ucvf.z;
            sspref3df.xy = (t22ucvf.xy * 0.5);
            highp vec3 j23kqa;
            #line 393
            j23kqa.xy = i.uv;
            j23kqa.z = efopafeod2s;
            len2ovaed = 0.0;
            highp float bberesa = (2.0 / _ScreenParams.x);
            #line 397
            highp float trdfgr25t = sqrt(dot( sspref3df.xy, sspref3df.xy));
            highp vec3 v26o9ij = (sspref3df * ((bberesa * _stepGlobalScale) / trdfgr25t));
            highp vec3 sam27pio;
            highp int mx29iujh = int(_maxStep);
            #line 401
            highp float ql30fg = len2ovaed;
            highp vec4 hheropg;
            bool boo32df = false;
            sam27pio = (j23kqa + v26o9ij);
            #line 405
            highp int i33tyux = 0;
            highp float tpv34gsf;
            highp float vrtoinhx;
            highp vec4 decrect36s;
            #line 409
            highp int s = 0;
            s = 0;
            for ( ; (s < 99); (s++)) {
                #line 414
                if ((i33tyux >= mx29iujh)){
                    break;
                }
                #line 418
                tpv34gsf = (1.0 / ((_ZBufferParams.x * xll_tex2Dlod( _depthTexCustom, vec4( sam27pio.xy, 0.0, 0.0)).x) + _ZBufferParams.y));
                vrtoinhx = (1.0 / ((_ZBufferParams.x * sam27pio.z) + _ZBufferParams.y));
                if ((tpv34gsf < (vrtoinhx - 1e-06))){
                    #line 422
                    decrect36s.w = 1.0;
                    decrect36s.xyz = sam27pio;
                    hheropg = decrect36s;
                    boo32df = true;
                    #line 426
                    break;
                }
                sam27pio = (sam27pio + v26o9ij);
                ql30fg = (ql30fg + 1.0);
                #line 430
                i33tyux = (i33tyux + 1);
            }
            if ((boo32df == false)){
                #line 434
                highp vec4 tpv37xs;
                tpv37xs.w = 0.0;
                tpv37xs.xyz = sam27pio;
                hheropg = tpv37xs;
                #line 438
                boo32df = true;
            }
            len2ovaed = ql30fg;
            resfinaelxe = hheropg;
            #line 442
            highp float tpv38xoi;
            tpv38xoi = abs((hheropg.x - 0.5));
            highp vec4 orgtpm4x = osdfej3;
            if ((_FlipReflectionsMSAA > 0.0)){
                #line 447
                highp vec2 tmpouv = i.uv;
                tmpouv.y = (1.0 - tmpouv.y);
                orgtpm4x = xll_tex2Dlod( _MainTex, vec4( tmpouv, 0.0, 0.0));
            }
            #line 451
            highp vec4 pacolac2s = vec4( 0.0, 0.0, 0.0, 0.0);
            if ((_SSRRcomposeMode > 0.0)){
                pacolac2s = vec4( orgtpm4x.xyz, 0.0);
            }
            if ((tpv38xoi > 0.5)){
                #line 455
                frefcol2d = pacolac2s;
            }
            else{
                #line 459
                highp float op39xcv = abs((hheropg.y - 0.5));
                if ((op39xcv > 0.5)){
                    frefcol2d = pacolac2s;
                }
                else{
                    #line 466
                    if (((1.0 / ((_ZBufferParams.x * hheropg.z) + _ZBufferParams.y)) > _maxDepthCull)){
                        frefcol2d = pacolac2s;
                    }
                    else{
                        #line 472
                        if ((hheropg.z < 0.1)){
                            frefcol2d = pacolac2s;
                        }
                        else{
                            #line 478
                            if ((hheropg.w == 1.0)){
                                highp vec3 v41yup = (hheropg.xyz - v26o9ij);
                                highp vec3 yipr42x = (sspref3df * (bberesa / trdfgr25t));
                                #line 482
                                highp vec3 ps43testy;
                                highp vec3 freg44r;
                                freg44r = yipr42x;
                                highp int mxc45ui = int(_maxFineStep);
                                #line 486
                                highp vec4 ghfjghtbbv;
                                bool oopplx = false;
                                highp int i49opght;
                                highp vec3 opl50op = v41yup;
                                #line 490
                                ps43testy = (v41yup + yipr42x);
                                i49opght = 0;
                                highp float vre51lv;
                                highp float lkde52xw;
                                #line 494
                                highp vec4 iotared;
                                highp vec3 yrrkjgf54t;
                                highp int j = 0;
                                j = 0;
                                for ( ; (j < 20); (j++)) {
                                    #line 501
                                    if ((i49opght >= mxc45ui)){
                                        break;
                                    }
                                    #line 505
                                    vre51lv = (1.0 / ((_ZBufferParams.x * xll_tex2Dlod( _depthTexCustom, vec4( ps43testy.xy, 0.0, 0.0)).x) + _ZBufferParams.y));
                                    lkde52xw = (1.0 / ((_ZBufferParams.x * ps43testy.z) + _ZBufferParams.y));
                                    if ((vre51lv < lkde52xw)){
                                        #line 509
                                        if (((lkde52xw - vre51lv) < _bias)){
                                            iotared.w = 1.0;
                                            iotared.xyz = ps43testy;
                                            #line 513
                                            ghfjghtbbv = iotared;
                                            oopplx = true;
                                            break;
                                        }
                                        #line 517
                                        yrrkjgf54t = (freg44r * 0.5);
                                        freg44r = yrrkjgf54t;
                                        ps43testy = (opl50op + yrrkjgf54t);
                                    }
                                    else{
                                        #line 523
                                        opl50op = ps43testy;
                                        ps43testy = (ps43testy + freg44r);
                                    }
                                    i49opght = (i49opght + 1);
                                }
                                #line 528
                                if ((oopplx == false)){
                                    highp vec4 vsap55f;
                                    vsap55f.w = 0.0;
                                    #line 532
                                    vsap55f.xyz = ps43testy;
                                    ghfjghtbbv = vsap55f;
                                    oopplx = true;
                                }
                                #line 536
                                resfinaelxe = ghfjghtbbv;
                            }
                            if ((resfinaelxe.w < 0.01)){
                                #line 540
                                frefcol2d = pacolac2s;
                            }
                            else{
                                #line 544
                                highp vec4 ui57tefrt;
                                highp vec2 tmpres3 = resfinaelxe.xy;
                                if ((_FlipReflectionsMSAA > 0.0)){
                                    #line 548
                                    resfinaelxe.y = (1.0 - resfinaelxe.y);
                                    tmpres3 = resfinaelxe.xy;
                                    tmpres3.y = (1.0 - tmpres3.y);
                                }
                                #line 552
                                if ((xll_tex2Dlod( _depthTexCustom, vec4( tmpres3.xy, 0.0, 0.0)).y > 0.1)){
                                    if ((_renderCustomColorMap < 0.5)){
                                        ui57tefrt.xyz = xll_tex2Dlod( _MainTex, vec4( resfinaelxe.xy, 0.0, 0.0)).xyz;
                                    }
                                    else{
                                        ui57tefrt.xyz = xll_tex2Dlod( _ColorTextureCustom, vec4( resfinaelxe.xy, 0.0, 0.0)).xyz;
                                    }
                                    #line 556
                                    ui57tefrt.w = (((resfinaelxe.w * (1.0 - (v11dflke / _maxDepthCull))) * (1.0 - pow( (ql30fg / float(maxcdfoief)), _fadePower))) * pow( clamp( ((dot( normalize(v19vvdss), normalize(v14kkmng).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                                    frefcol2d = ui57tefrt;
                                }
                                else{
                                    #line 561
                                    frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #line 570
    return frefcol2d;
}
in highp vec2 xlv_TEXCOORD0;
void main() {
    mediump vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   d3d9 - ALU: 195 to 195, TEX: 18 to 18, FLOW: 30 to 30
//   d3d11 - ALU: 112 to 112, TEX: 0 to 0, FLOW: 34 to 34
SubProgram "opengl " {
Keywords { }
"!!GLSL"
}

SubProgram "d3d9 " {
Keywords { }
Vector 12 [_ScreenParams]
Vector 13 [_ZBufferParams]
Float 14 [_fadePower]
Float 15 [_maxDepthCull]
Float 16 [_maxFineStep]
Float 17 [_maxStep]
Float 18 [_stepGlobalScale]
Float 19 [_bias]
Matrix 0 [_ProjMatrix]
Matrix 4 [_ProjectionInv]
Matrix 8 [_ViewMatrix]
Float 20 [_SSRRcomposeMode]
Float 21 [_FlipReflectionsMSAA]
Float 22 [_renderCustomColorMap]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_depthTexCustom] 2D
SetTexture 2 [_CameraNormalsTexture] 2D
SetTexture 3 [_ColorTextureCustom] 2D
"ps_3_0
; 195 ALU, 18 TEX, 30 FLOW
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c23, 0.00000000, 2.00000000, -1.00000000, 1.00000000
def c24, 0.50000000, 1.00000000, -0.00000100, -0.50000000
defi i0, 99, 0, 1, 0
def c25, 0.10000000, 0.01000000, 0, 0
defi i1, 20, 0, 1, 0
dcl_texcoord0 v0.xy
mov r0.xy, v0
mov r0.z, c23.x
texldl r3, r0.xyzz, s0
if_eq r3.w, c23.x
mov r0, c23.x
else
mov r0.xy, v0
mov r0.z, c23.x
texldl r0.x, r0.xyzz, s1
mad r0.y, r0.x, c13.x, c13
rcp r4.w, r0.y
if_gt r4.w, c15.x
mov r0, c23.x
else
mad r8.xy, v0, c23.y, c23.z
mov r5.z, r0.x
mov r5.xy, r8
mov r5.w, c23
dp4 r0.y, r5, c7
mov r1.w, r0.y
mov r6.w, c23
dp4 r1.z, r5, c6
dp4 r1.y, r5, c5
dp4 r1.x, r5, c4
rcp r0.y, r0.y
mul r1, r1, r0.y
dp3 r0.z, r1, r1
mov r5.w, c23.x
mov r8.z, r0.x
rsq r0.z, r0.z
mov r5.z, c23.x
mov r5.xy, v0
texldl r5.xyz, r5.xyzz, s2
mad r5.xyz, r5, c23.y, c23.z
dp4 r6.z, r5, c10
dp4 r6.x, r5, c8
dp4 r6.y, r5, c9
dp3 r0.y, r6, r6
rsq r0.y, r0.y
mul r5.xyz, r0.z, r1
mul r6.xyz, r0.y, r6
dp3 r0.y, r6, r5
mul r6.xyz, r0.y, r6
mad r5.xyz, -r6, c23.y, r5
dp3 r0.y, r5, r5
rsq r0.y, r0.y
mul r5.xyz, r0.y, r5
add r6.xyz, r1, r5
dp4 r0.y, r6, c3
dp4 r7.z, r6, c2
dp4 r7.y, r6, c1
dp4 r7.x, r6, c0
rcp r0.y, r0.y
mad r6.xyz, r7, r0.y, -r8
dp3 r0.y, r6, r6
rsq r0.y, r0.y
mul r6.xyz, r0.y, r6
mul r0.zw, r6.xyxy, c24.x
rcp r0.y, c12.x
mul r0.zw, r0, r0
mul r7.w, r0.y, c23.y
add r0.y, r0.z, r0.w
rsq r5.w, r0.y
mul r0.z, r7.w, c18.x
mul r0.y, r5.w, r0.z
abs r0.w, c17.x
frc r3.w, r0
add r0.w, r0, -r3
mul r6.xyz, r6, c24.xxyw
mul r7.xyz, r6, r0.y
mov r0.z, r0.x
mov r0.xy, v0
add r8.xyz, r7, r0
cmp r0.x, c17, r0.w, -r0.w
rcp r8.w, r5.w
mov r5.w, r0.x
mov r9.x, r0
mov r6.w, c23.x
mov_pp r3.w, c23.x
mov r9.y, c23.x
loop aL, i0
break_ge r9.y, r9.x
mad r0.w, r8.z, c13.x, c13.y
mov r0.z, c23.x
mov r0.xy, r8
texldl r0.x, r0.xyzz, s1
rcp r0.y, r0.w
mad r0.x, r0, c13, c13.y
add r9.w, r0.y, c24.z
rcp r9.z, r0.x
add r10.x, r9.z, -r9.w
mov r0.xyz, r8
mov r0.w, c23
cmp r2, r10.x, r2, r0
cmp_pp r3.w, r10.x, r3, c23
break_lt r9.z, r9.w
add r8.xyz, r8, r7
add r6.w, r6, c23
add r9.y, r9, c23.w
endloop
abs_pp r3.w, r3
mov r0.xyz, r8
mov r0.w, c23.x
cmp r0, -r3.w, r0, r2
add r3.w, r0.x, c24
mov r2, r0
mov r0.xyz, r0.xyww
abs r0.w, r3
mov r3.w, c23.x
if_gt c21.x, r3.w
mov r3.x, v0
mov r3.z, c23.x
add r3.y, -v0, c23.w
texldl r3.xyz, r3.xyzz, s0
endif
mov r3.w, c23.x
mov r8.x, c23
cmp r3, -c20.x, r8.x, r3
if_gt r0.w, c24.x
mov r0, r3
else
add r0.w, r2.y, c24
abs r0.w, r0
if_gt r0.w, c24.x
mov r0, r3
else
mad r0.w, r2.z, c13.x, c13.y
rcp r0.w, r0.w
if_gt r0.w, c15.x
mov r0, r3
else
if_lt r2.z, c25.x
mov r0, r3
else
if_eq r2.w, c23.w
abs r0.y, c16.x
rcp r0.x, r8.w
mul r0.x, r7.w, r0
mul r6.xyz, r6, r0.x
add r2.xyz, r2, -r7
frc r0.z, r0.y
add r0.x, r0.y, -r0.z
add r7.xyz, r6, r2
cmp r2.w, c16.x, r0.x, -r0.x
mov_pp r0.w, c23.x
mov r7.w, c23.x
loop aL, i1
break_ge r7.w, r2.w
mov r0.z, c23.x
mov r0.xy, r7
texldl r0.x, r0.xyzz, s1
mad r0.y, r7.z, c13.x, c13
mad r0.x, r0, c13, c13.y
rcp r0.y, r0.y
rcp r0.x, r0.x
add r0.z, -r0.x, r0.y
add r0.x, r0, -r0.y
add r0.z, r0, -c19.x
cmp r0.y, r0.z, c23.x, c23.w
cmp r8.w, r0.x, c23.x, c23
mul_pp r8.x, r8.w, r0.y
mov r0.xy, r7
mov r0.z, c23.w
cmp r4.xyz, -r8.x, r4, r0
cmp_pp r0.w, -r8.x, r0, c23
break_gt r8.x, c23.x
mul r0.xyz, r6, c24.x
add r8.xyz, r0, r2
cmp r8.xyz, -r8.w, r7, r8
cmp r6.xyz, -r8.w, r6, r0
abs_pp r8.w, r8
add r0.xyz, r6, r8
cmp r7.xyz, -r8.w, r0, r8
cmp r2.xyz, -r8.w, r8, r2
add r7.w, r7, c23
endloop
mov r0.xy, r7
mov r0.z, c23.x
abs_pp r0.w, r0
cmp r0.xyz, -r0.w, r0, r4
endif
if_lt r0.z, c25.y
mov r0, r3
else
mov r3.xw, r0.xyzz
add r0.w, -r0.y, c23
cmp r0.w, -c21.x, r0.y, r0
mov r3.y, r0.w
cmp r0.xy, -c21.x, r0, r3
add r0.w, -r0, c23
mov r0.z, c23.x
cmp r0.y, -c21.x, r0, r0.w
texldl r2.y, r0.xyzz, s1
mov r0.xyz, r3.xyww
if_gt r2.y, c25.x
mov r0.w, c24.x
if_lt c22.x, r0.w
mov r2.z, c23.x
mov r2.xy, r0
texldl r2.xyz, r2.xyzz, s0
else
mov r2.z, c23.x
mov r2.xy, r0
texldl r2.xyz, r2.xyzz, s3
endif
dp4 r0.y, r1, r1
rsq r0.y, r0.y
dp3 r0.x, r5, r5
mul r3.xyz, r0.y, r1
rsq r0.x, r0.x
mul r1.xyz, r0.x, r5
dp3 r0.y, r1, r3
mov r0.x, c14
mad r0.x, c25, r0, r0.y
add_sat r0.y, r0.x, c23.w
pow r1, r0.y, c14.x
rcp r0.x, r5.w
mul r0.x, r6.w, r0
pow r3, r0.x, c14.x
rcp r0.x, c15.x
mov r0.y, r3.x
mad r0.x, -r4.w, r0, c23.w
add r0.y, -r0, c23.w
mul r0.x, r0.z, r0
mul r0.x, r0, r0.y
mov r0.w, r1.x
mul r0.w, r0.x, r0
mov r0.xyz, r2
else
mov r0, c23.x
endif
endif
endif
endif
endif
endif
endif
endif
mov_pp oC0, r0
"
}

SubProgram "d3d11 " {
Keywords { }
ConstBuffer "$Globals" 272 // 268 used size, 14 vars
Float 16 [_fadePower]
Float 20 [_maxDepthCull]
Float 24 [_maxFineStep]
Float 28 [_maxStep]
Float 32 [_stepGlobalScale]
Float 36 [_bias]
Matrix 48 [_ProjMatrix] 4
Matrix 112 [_ProjectionInv] 4
Matrix 176 [_ViewMatrix] 4
Float 256 [_SSRRcomposeMode]
Float 260 [_FlipReflectionsMSAA]
Float 264 [_renderCustomColorMap]
ConstBuffer "UnityPerCamera" 128 // 128 used size, 8 vars
Vector 96 [_ScreenParams] 4
Vector 112 [_ZBufferParams] 4
BindCB "$Globals" 0
BindCB "UnityPerCamera" 1
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_depthTexCustom] 2D 0
SetTexture 2 [_CameraNormalsTexture] 2D 2
SetTexture 3 [_ColorTextureCustom] 2D 3
// 221 instructions, 14 temp regs, 0 temp arrays:
// ALU 103 float, 8 int, 1 uint
// TEX 0 (9 load, 0 comp, 0 bias, 0 grad)
// FLOW 16 static, 18 dynamic
"ps_4_0
eefiecedppeagbgchmlkjnhgndgcaboofchccgdcabaaaaaapebgaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcdebgaaaa
eaaaaaaainafaaaafjaaaaaeegiocaaaaaaaaaaabbaaaaaafjaaaaaeegiocaaa
abaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafkaaaaadaagabaaaadaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaafibiaaaeaahabaaa
acaaaaaaffffaaaafibiaaaeaahabaaaadaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacaoaaaaaaeiaaaaalpcaabaaa
aaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaabeaaaaa
aaaaaaaabiaaaaahbcaabaaaabaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaaa
bpaaaeadakaabaaaabaaaaaadgaaaaaipccabaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaabcaaaaabeiaaaaalpcaabaaaabaaaaaaegbabaaa
abaaaaaajghmbaaaabaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaal
icaabaaaabaaaaaaakiacaaaabaaaaaaahaaaaaackaabaaaabaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakicaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpdkaabaaaabaaaaaadbaaaaaibcaabaaaacaaaaaabkiacaaa
aaaaaaaaabaaaaaadkaabaaaabaaaaaabpaaaeadakaabaaaacaaaaaadgaaaaai
pccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabcaaaaab
dcaaaaapdcaabaaaacaaaaaaegbabaaaabaaaaaaaceaaaaaaaaaaaeaaaaaaaea
aaaaaaaaaaaaaaaaaceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaadiaaaaai
pcaabaaaadaaaaaafgafbaaaacaaaaaaegiocaaaaaaaaaaaaiaaaaaadcaaaaak
pcaabaaaacaaaaaaegiocaaaaaaaaaaaahaaaaaaagaabaaaacaaaaaaegaobaaa
adaaaaaadcaaaaakpcaabaaaacaaaaaaegiocaaaaaaaaaaaajaaaaaakgakbaaa
abaaaaaaegaobaaaacaaaaaaaaaaaaaipcaabaaaacaaaaaaegaobaaaacaaaaaa
egiocaaaaaaaaaaaakaaaaaaaoaaaaahpcaabaaaacaaaaaaegaobaaaacaaaaaa
pgapbaaaacaaaaaaeiaaaaalpcaabaaaadaaaaaaegbabaaaabaaaaaaeghobaaa
acaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaadcaaaaaphcaabaaaadaaaaaa
egacbaaaadaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaaceaaaaa
aaaaialpaaaaialpaaaaialpaaaaaaaabaaaaaahicaabaaaadaaaaaaegacbaaa
acaaaaaaegacbaaaacaaaaaaeeaaaaaficaabaaaadaaaaaadkaabaaaadaaaaaa
diaaaaahhcaabaaaaeaaaaaaegacbaaaacaaaaaapgapbaaaadaaaaaadiaaaaai
hcaabaaaafaaaaaafgafbaaaadaaaaaaegiccaaaaaaaaaaaamaaaaaadcaaaaak
lcaabaaaadaaaaaaegiicaaaaaaaaaaaalaaaaaaagaabaaaadaaaaaaegaibaaa
afaaaaaadcaaaaakhcaabaaaadaaaaaaegiccaaaaaaaaaaaanaaaaaakgakbaaa
adaaaaaaegadbaaaadaaaaaabaaaaaahicaabaaaadaaaaaaegacbaaaadaaaaaa
egacbaaaadaaaaaaeeaaaaaficaabaaaadaaaaaadkaabaaaadaaaaaadiaaaaah
hcaabaaaadaaaaaapgapbaaaadaaaaaaegacbaaaadaaaaaabaaaaaahicaabaaa
adaaaaaaegacbaaaadaaaaaaegacbaaaaeaaaaaadiaaaaahhcaabaaaadaaaaaa
egacbaaaadaaaaaapgapbaaaadaaaaaadcaaaaanhcaabaaaadaaaaaaegacbaia
ebaaaaaaadaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaegacbaaa
aeaaaaaabaaaaaahicaabaaaadaaaaaaegacbaaaadaaaaaaegacbaaaadaaaaaa
eeaaaaaficaabaaaadaaaaaadkaabaaaadaaaaaadiaaaaahhcaabaaaaeaaaaaa
pgapbaaaadaaaaaaegacbaaaadaaaaaadcaaaaajhcaabaaaadaaaaaaegacbaaa
adaaaaaapgapbaaaadaaaaaaegacbaaaacaaaaaadiaaaaaipcaabaaaafaaaaaa
fgafbaaaadaaaaaaegiocaaaaaaaaaaaaeaaaaaadcaaaaakpcaabaaaafaaaaaa
egiocaaaaaaaaaaaadaaaaaaagaabaaaadaaaaaaegaobaaaafaaaaaadcaaaaak
pcaabaaaadaaaaaaegiocaaaaaaaaaaaafaaaaaakgakbaaaadaaaaaaegaobaaa
afaaaaaaaaaaaaaipcaabaaaadaaaaaaegaobaaaadaaaaaaegiocaaaaaaaaaaa
agaaaaaaaoaaaaahhcaabaaaadaaaaaaegacbaaaadaaaaaapgapbaaaadaaaaaa
dcaaaaapdcaabaaaabaaaaaaegbabaaaabaaaaaaaceaaaaaaaaaaaeaaaaaaaea
aaaaaaaaaaaaaaaaaceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaaaaaaaaai
hcaabaaaadaaaaaaegacbaiaebaaaaaaabaaaaaaegacbaaaadaaaaaabaaaaaah
icaabaaaadaaaaaaegacbaaaadaaaaaaegacbaaaadaaaaaaeeaaaaaficaabaaa
adaaaaaadkaabaaaadaaaaaadiaaaaahhcaabaaaadaaaaaapgapbaaaadaaaaaa
egacbaaaadaaaaaadiaaaaakdcaabaaaafaaaaaaegaabaaaadaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaaaaaaaaaaaaaoaaaaaiicaabaaaadaaaaaaabeaaaaa
aaaaaaeaakiacaaaabaaaaaaagaaaaaaapaaaaahicaabaaaaeaaaaaaegaabaaa
afaaaaaaegaabaaaafaaaaaaelaaaaaficaabaaaaeaaaaaadkaabaaaaeaaaaaa
diaaaaaibcaabaaaafaaaaaadkaabaaaadaaaaaaakiacaaaaaaaaaaaacaaaaaa
aoaaaaahbcaabaaaafaaaaaaakaabaaaafaaaaaadkaabaaaaeaaaaaadiaaaaak
hcaabaaaadaaaaaaegacbaaaadaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaiadp
aaaaaaaablaaaaagccaabaaaafaaaaaadkiacaaaaaaaaaaaabaaaaaadgaaaaaf
dcaabaaaabaaaaaaegbabaaaabaaaaaadcaaaaajhcaabaaaabaaaaaaegacbaaa
adaaaaaaagaabaaaafaaaaaaegacbaaaabaaaaaadgaaaaaficaabaaaagaaaaaa
abeaaaaaaaaaiadpdgaaaaaipcaabaaaahaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaadgaaaaafhcaabaaaagaaaaaaegacbaaaabaaaaaadgaaaaai
mcaabaaaafaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaai
dcaabaaaaiaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadaaaaaab
cbaaaaahecaabaaaaiaaaaaabkaabaaaaiaaaaaaabeaaaaagdaaaaaaadaaaead
ckaabaaaaiaaaaaacbaaaaahecaabaaaaiaaaaaaakaabaaaaiaaaaaabkaabaaa
afaaaaaabpaaaeadckaabaaaaiaaaaaaacaaaaabbfaaaaabeiaaaaalpcaabaaa
ajaaaaaaegaabaaaagaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaadcaaaaalecaabaaaaiaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaa
ajaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakecaabaaaaiaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpckaabaaaaiaaaaaadcaaaaalicaabaaa
aiaaaaaaakiacaaaabaaaaaaahaaaaaackaabaaaagaaaaaabkiacaaaabaaaaaa
ahaaaaaaaoaaaaakicaabaaaaiaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpdkaabaaaaiaaaaaaaaaaaaahicaabaaaaiaaaaaadkaabaaaaiaaaaaa
abeaaaaalndhiglfdbaaaaahecaabaaaaiaaaaaackaabaaaaiaaaaaadkaabaaa
aiaaaaaabpaaaeadckaabaaaaiaaaaaadgaaaaafpcaabaaaahaaaaaaegaobaaa
agaaaaaadgaaaaaficaabaaaafaaaaaaabeaaaaappppppppacaaaaabbfaaaaab
dcaaaaajhcaabaaaagaaaaaaegacbaaaadaaaaaaagaabaaaafaaaaaaegacbaaa
agaaaaaaaaaaaaahecaabaaaafaaaaaackaabaaaafaaaaaaabeaaaaaaaaaiadp
boaaaaahbcaabaaaaiaaaaaaakaabaaaaiaaaaaaabeaaaaaabaaaaaaboaaaaah
ccaabaaaaiaaaaaabkaabaaaaiaaaaaaabeaaaaaabaaaaaadgaaaaaipcaabaaa
ahaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaaficaabaaa
afaaaaaaabeaaaaaaaaaaaaabgaaaaabdgaaaaaficaabaaaagaaaaaaabeaaaaa
aaaaaaaadhaaaaajpcaabaaaagaaaaaapgapbaaaafaaaaaaegaobaaaahaaaaaa
egaobaaaagaaaaaaaaaaaaahbcaabaaaabaaaaaaakaabaaaagaaaaaaabeaaaaa
aaaaaalpdbaaaaalgcaabaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaafgiecaaaaaaaaaaabaaaaaaabpaaaeadbkaabaaaabaaaaaadcaaaaap
kcaabaaaafaaaaaaagbebaaaabaaaaaaaceaaaaaaaaaaaaaaaaaiadpaaaaaaaa
aaaaialpaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiadpeiaaaaalpcaabaaa
aaaaaaaangafbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaabeaaaaa
aaaaaaaabfaaaaababaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaakgakbaaa
abaaaaaadgaaaaaficaabaaaaaaaaaaaabeaaaaaaaaaaaaadbaaaaaibcaabaaa
abaaaaaaabeaaaaaaaaaaadpakaabaiaibaaaaaaabaaaaaabpaaaeadakaabaaa
abaaaaaadgaaaaafpccabaaaaaaaaaaaegaobaaaaaaaaaaabcaaaaabaaaaaaah
bcaabaaaabaaaaaabkaabaaaagaaaaaaabeaaaaaaaaaaalpdbaaaaaibcaabaaa
abaaaaaaabeaaaaaaaaaaadpakaabaiaibaaaaaaabaaaaaabpaaaeadakaabaaa
abaaaaaadgaaaaafpccabaaaaaaaaaaaegaobaaaaaaaaaaabcaaaaabdcaaaaal
bcaabaaaabaaaaaaakiacaaaabaaaaaaahaaaaaackaabaaaagaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakbcaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpakaabaaaabaaaaaadbaaaaaibcaabaaaabaaaaaabkiacaaa
aaaaaaaaabaaaaaaakaabaaaabaaaaaabpaaaeadakaabaaaabaaaaaadgaaaaaf
pccabaaaaaaaaaaaegaobaaaaaaaaaaabcaaaaabdbaaaaahbcaabaaaabaaaaaa
ckaabaaaagaaaaaaabeaaaaamnmmmmdnbpaaaeadakaabaaaabaaaaaadgaaaaaf
pccabaaaaaaaaaaaegaobaaaaaaaaaaabcaaaaabbiaaaaahbcaabaaaabaaaaaa
dkaabaaaagaaaaaaabeaaaaaaaaaiadpbpaaaeadakaabaaaabaaaaaadcaaaaak
lcaabaaaafaaaaaaegaibaiaebaaaaaaadaaaaaaagaabaaaafaaaaaaegaibaaa
agaaaaaaaoaaaaahbcaabaaaabaaaaaadkaabaaaadaaaaaadkaabaaaaeaaaaaa
diaaaaahhcaabaaaahaaaaaaagaabaaaabaaaaaaegacbaaaadaaaaaablaaaaag
ecaabaaaabaaaaaackiacaaaaaaaaaaaabaaaaaadcaaaaajhcaabaaaadaaaaaa
egacbaaaadaaaaaaagaabaaaabaaaaaaegadbaaaafaaaaaadgaaaaafecaabaaa
aiaaaaaaabeaaaaaaaaaiadpdgaaaaafhcaabaaaajaaaaaaegacbaaaahaaaaaa
dgaaaaaihcaabaaaakaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
dgaaaaafhcaabaaaalaaaaaaegadbaaaafaaaaaadgaaaaafhcaabaaaamaaaaaa
egacbaaaadaaaaaadgaaaaafbcaabaaaabaaaaaaabeaaaaaaaaaaaaadgaaaaaf
icaabaaaadaaaaaaabeaaaaaaaaaaaaadgaaaaaficaabaaaaeaaaaaaabeaaaaa
aaaaaaaadaaaaaabcbaaaaahicaabaaaagaaaaaadkaabaaaaeaaaaaaabeaaaaa
beaaaaaaadaaaeaddkaabaaaagaaaaaacbaaaaahicaabaaaagaaaaaadkaabaaa
adaaaaaackaabaaaabaaaaaabpaaaeaddkaabaaaagaaaaaaacaaaaabbfaaaaab
eiaaaaalpcaabaaaanaaaaaaegaabaaaamaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadcaaaaalicaabaaaagaaaaaaakiacaaaabaaaaaa
ahaaaaaaakaabaaaanaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaa
agaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaagaaaaaa
dcaaaaalicaabaaaahaaaaaaakiacaaaabaaaaaaahaaaaaackaabaaaamaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaaahaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpdkaabaaaahaaaaaadbaaaaahicaabaaaaiaaaaaa
dkaabaaaagaaaaaadkaabaaaahaaaaaabpaaaeaddkaabaaaaiaaaaaaaaaaaaai
icaabaaaagaaaaaadkaabaiaebaaaaaaagaaaaaadkaabaaaahaaaaaadbaaaaai
icaabaaaagaaaaaadkaabaaaagaaaaaabkiacaaaaaaaaaaaacaaaaaabpaaaead
dkaabaaaagaaaaaadgaaaaafdcaabaaaaiaaaaaaegaabaaaamaaaaaadgaaaaaf
hcaabaaaakaaaaaaegacbaaaaiaaaaaadgaaaaafbcaabaaaabaaaaaaabeaaaaa
ppppppppacaaaaabbfaaaaabdiaaaaaklcaabaaaaiaaaaaaegaibaaaajaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaadpdcaaaaamhcaabaaaamaaaaaa
egacbaaaajaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaaaaegacbaaa
alaaaaaadgaaaaafhcaabaaaajaaaaaaegadbaaaaiaaaaaabcaaaaabaaaaaaah
lcaabaaaaiaaaaaaegaibaaaajaaaaaaegaibaaaamaaaaaadgaaaaafhcaabaaa
alaaaaaaegacbaaaamaaaaaadgaaaaafhcaabaaaamaaaaaaegadbaaaaiaaaaaa
bfaaaaabboaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaaabeaaaaaabaaaaaa
boaaaaahicaabaaaaeaaaaaadkaabaaaaeaaaaaaabeaaaaaabaaaaaadgaaaaai
hcaabaaaakaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaaf
bcaabaaaabaaaaaaabeaaaaaaaaaaaaabgaaaaabdgaaaaafecaabaaaamaaaaaa
abeaaaaaaaaaaaaadhaaaaajhcaabaaaadaaaaaaagaabaaaabaaaaaacgajbaaa
akaaaaaacgajbaaaamaaaaaadgaaaaafdcaabaaaagaaaaaajgafbaaaadaaaaaa
bcaaaaabdgaaaaafbcaabaaaadaaaaaaabeaaaaaaaaaaaaabfaaaaabdbaaaaah
bcaabaaaabaaaaaaakaabaaaadaaaaaaabeaaaaaaknhcddmbpaaaeadakaabaaa
abaaaaaadgaaaaafpccabaaaaaaaaaaaegaobaaaaaaaaaaabcaaaaabaaaaaaai
ccaabaaaaaaaaaaabkaabaiaebaaaaaaagaaaaaaabeaaaaaaaaaiadpaaaaaaai
ecaabaaaaaaaaaaabkaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdhaaaaaj
gcaabaaaagaaaaaafgafbaaaabaaaaaafgagbaaaaaaaaaaafgafbaaaagaaaaaa
eiaaaaalpcaabaaaaaaaaaaaigaabaaaagaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadbaaaaahbcaabaaaaaaaaaaaabeaaaaamnmmmmdn
bkaabaaaaaaaaaaabpaaaeadakaabaaaaaaaaaaadbaaaaaibcaabaaaaaaaaaaa
ckiacaaaaaaaaaaabaaaaaaaabeaaaaaaaaaaadpbpaaaeadakaabaaaaaaaaaaa
eiaaaaalpcaabaaaaaaaaaaaegaabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaa
abaaaaaaabeaaaaaaaaaaaaadgaaaaafhccabaaaaaaaaaaaegacbaaaaaaaaaaa
bcaaaaabeiaaaaalpcaabaaaaaaaaaaaegaabaaaagaaaaaaeghobaaaadaaaaaa
aagabaaaadaaaaaaabeaaaaaaaaaaaaadgaaaaafhccabaaaaaaaaaaaegacbaaa
aaaaaaaabfaaaaabaoaaaaaibcaabaaaaaaaaaaadkaabaaaabaaaaaabkiacaaa
aaaaaaaaabaaaaaaaaaaaaaibcaabaaaaaaaaaaaakaabaiaebaaaaaaaaaaaaaa
abeaaaaaaaaaiadpdiaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaakaabaaa
adaaaaaaedaaaaagccaabaaaaaaaaaaadkiacaaaaaaaaaaaabaaaaaaaoaaaaah
ccaabaaaaaaaaaaackaabaaaafaaaaaabkaabaaaaaaaaaaacpaaaaafccaabaaa
aaaaaaaabkaabaaaaaaaaaaadiaaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaa
akiacaaaaaaaaaaaabaaaaaabjaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadp
diaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaabbaaaaah
ccaabaaaaaaaaaaaegaobaaaacaaaaaaegaobaaaacaaaaaaeeaaaaafccaabaaa
aaaaaaaabkaabaaaaaaaaaaadiaaaaahocaabaaaaaaaaaaafgafbaaaaaaaaaaa
agajbaaaacaaaaaabaaaaaahccaabaaaaaaaaaaaegacbaaaaeaaaaaajgahbaaa
aaaaaaaaaaaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaaaaaaiadp
dccaaaakccaabaaaaaaaaaaaakiacaaaaaaaaaaaabaaaaaaabeaaaaamnmmmmdn
bkaabaaaaaaaaaaacpaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaadiaaaaai
ccaabaaaaaaaaaaabkaabaaaaaaaaaaaakiacaaaaaaaaaaaabaaaaaabjaaaaaf
ccaabaaaaaaaaaaabkaabaaaaaaaaaaadiaaaaahiccabaaaaaaaaaaabkaabaaa
aaaaaaaaakaabaaaaaaaaaaabcaaaaabdgaaaaaipccabaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabfaaaaabbfaaaaabbfaaaaabbfaaaaab
bfaaaaabbfaaaaabbfaaaaabbfaaaaabdoaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 347

	}
	
	//#################################################################################################################################################
	//================================================================================================================================================
	//USING UNITY DEPTH TEXTURE
	//================================================================================================================================================
	Pass {
				Program "vp" {
// Vertex combos: 1
//   d3d9 - ALU: 5 to 5
//   d3d11 - ALU: 4 to 4, TEX: 0 to 0, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
"!!GLSL
#ifdef VERTEX
varying vec2 xlv_TEXCOORD0;

void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
varying vec2 xlv_TEXCOORD0;
uniform float _SSRRcomposeMode;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _CameraNormalsTexture;
uniform mat4 _ViewMatrix;
uniform mat4 _ProjectionInv;
uniform mat4 _ProjMatrix;
uniform float _bias;
uniform float _stepGlobalScale;
uniform float _maxStep;
uniform float _maxFineStep;
uniform float _maxDepthCull;
uniform float _fadePower;
uniform sampler2D _MainTex;
uniform vec4 _ZBufferParams;
uniform vec4 _ScreenParams;
void main ()
{
  vec3 sspref3df_1;
  vec4 resfinaelxe_2;
  vec4 frefcol2d_3;
  vec4 tmpvar_4;
  tmpvar_4 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  if ((tmpvar_4.w == 0.0)) {
    frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
  } else {
    vec4 tmpvar_5;
    tmpvar_5 = texture2DLod (_CameraDepthTexture, xlv_TEXCOORD0, 0.0);
    float tmpvar_6;
    tmpvar_6 = tmpvar_5.x;
    float tmpvar_7;
    tmpvar_7 = (1.0/(((_ZBufferParams.x * tmpvar_5.x) + _ZBufferParams.y)));
    if ((tmpvar_7 > _maxDepthCull)) {
      frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
      vec4 pacolac2s_8;
      int s_9;
      vec4 decrect36s_10;
      int i33tyux_11;
      bool boo32df_12;
      vec4 hheropg_13;
      float ql30fg_14;
      int mx29iujh_15;
      vec3 sam27pio_16;
      vec3 v26o9ij_17;
      vec3 j23kqa_18;
      vec4 ght43s_19;
      vec4 nmgghg16y_20;
      vec3 oy15df_21;
      vec4 v12fefsk_22;
      int tmpvar_23;
      tmpvar_23 = int(_maxStep);
      v12fefsk_22.w = 1.0;
      v12fefsk_22.xy = ((xlv_TEXCOORD0 * 2.0) - 1.0);
      v12fefsk_22.z = tmpvar_6;
      vec4 tmpvar_24;
      tmpvar_24 = (_ProjectionInv * v12fefsk_22);
      vec4 tmpvar_25;
      tmpvar_25 = (tmpvar_24 / tmpvar_24.w);
      oy15df_21.xy = v12fefsk_22.xy;
      oy15df_21.z = tmpvar_6;
      nmgghg16y_20.w = 0.0;
      nmgghg16y_20.xyz = ((texture2DLod (_CameraNormalsTexture, xlv_TEXCOORD0, 0.0).xyz * 2.0) - 1.0);
      vec3 tmpvar_26;
      tmpvar_26 = normalize(tmpvar_25.xyz);
      vec3 tmpvar_27;
      tmpvar_27 = normalize((_ViewMatrix * nmgghg16y_20).xyz);
      vec3 tmpvar_28;
      tmpvar_28 = normalize((tmpvar_26 - (2.0 * (dot (tmpvar_27, tmpvar_26) * tmpvar_27))));
      ght43s_19.w = 1.0;
      ght43s_19.xyz = (tmpvar_25.xyz + tmpvar_28);
      vec4 tmpvar_29;
      tmpvar_29 = (_ProjMatrix * ght43s_19);
      vec3 tmpvar_30;
      tmpvar_30 = normalize(((tmpvar_29.xyz / tmpvar_29.w) - oy15df_21));
      sspref3df_1.z = tmpvar_30.z;
      sspref3df_1.xy = (tmpvar_30.xy * 0.5);
      j23kqa_18.xy = xlv_TEXCOORD0;
      j23kqa_18.z = tmpvar_6;
      float tmpvar_31;
      tmpvar_31 = (2.0 / _ScreenParams.x);
      float tmpvar_32;
      tmpvar_32 = sqrt(dot (sspref3df_1.xy, sspref3df_1.xy));
      vec3 tmpvar_33;
      tmpvar_33 = (sspref3df_1 * ((tmpvar_31 * _stepGlobalScale) / tmpvar_32));
      v26o9ij_17 = tmpvar_33;
      mx29iujh_15 = int(_maxStep);
      ql30fg_14 = 0.0;
      boo32df_12 = bool(0);
      sam27pio_16 = (j23kqa_18 + tmpvar_33);
      i33tyux_11 = 0;
      s_9 = 0;
      for (int s_9 = 0; s_9 < 99; ) {
        if ((i33tyux_11 >= mx29iujh_15)) {
          break;
        };
        float tmpvar_34;
        tmpvar_34 = (1.0/(((_ZBufferParams.x * texture2DLod (_CameraDepthTexture, sam27pio_16.xy, 0.0).x) + _ZBufferParams.y)));
        float tmpvar_35;
        tmpvar_35 = (1.0/(((_ZBufferParams.x * sam27pio_16.z) + _ZBufferParams.y)));
        if ((tmpvar_34 < (tmpvar_35 - 1e-06))) {
          decrect36s_10.w = 1.0;
          decrect36s_10.xyz = sam27pio_16;
          hheropg_13 = decrect36s_10;
          boo32df_12 = bool(1);
          break;
        };
        sam27pio_16 = (sam27pio_16 + v26o9ij_17);
        ql30fg_14 = (ql30fg_14 + 1.0);
        i33tyux_11 = (i33tyux_11 + 1);
        s_9 = (s_9 + 1);
      };
      if ((boo32df_12 == bool(0))) {
        vec4 tpv37xs_36;
        tpv37xs_36.w = 0.0;
        tpv37xs_36.xyz = sam27pio_16;
        hheropg_13 = tpv37xs_36;
        boo32df_12 = bool(1);
      };
      resfinaelxe_2 = hheropg_13;
      float tmpvar_37;
      tmpvar_37 = abs((hheropg_13.x - 0.5));
      pacolac2s_8 = vec4(0.0, 0.0, 0.0, 0.0);
      if ((_SSRRcomposeMode > 0.0)) {
        vec4 tmpvar_38;
        tmpvar_38.w = 0.0;
        tmpvar_38.xyz = tmpvar_4.xyz;
        pacolac2s_8 = tmpvar_38;
      };
      if ((tmpvar_37 > 0.5)) {
        frefcol2d_3 = pacolac2s_8;
      } else {
        float tmpvar_39;
        tmpvar_39 = abs((hheropg_13.y - 0.5));
        if ((tmpvar_39 > 0.5)) {
          frefcol2d_3 = pacolac2s_8;
        } else {
          if (((1.0/(((_ZBufferParams.x * hheropg_13.z) + _ZBufferParams.y))) > _maxDepthCull)) {
            frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
          } else {
            if ((hheropg_13.z < 0.1)) {
              frefcol2d_3 = vec4(0.0, 0.0, 0.0, 0.0);
            } else {
              if ((hheropg_13.w == 1.0)) {
                int j_40;
                vec4 iotared_41;
                vec3 opl50op_42;
                int i49opght_43;
                bool oopplx_44;
                vec4 ghfjghtbbv_45;
                int mxc45ui_46;
                vec3 freg44r_47;
                vec3 ps43testy_48;
                vec3 tmpvar_49;
                tmpvar_49 = (hheropg_13.xyz - tmpvar_33);
                vec3 tmpvar_50;
                tmpvar_50 = (sspref3df_1 * (tmpvar_31 / tmpvar_32));
                freg44r_47 = tmpvar_50;
                mxc45ui_46 = int(_maxFineStep);
                oopplx_44 = bool(0);
                opl50op_42 = tmpvar_49;
                ps43testy_48 = (tmpvar_49 + tmpvar_50);
                i49opght_43 = 0;
                j_40 = 0;
                for (int j_40 = 0; j_40 < 20; ) {
                  if ((i49opght_43 >= mxc45ui_46)) {
                    break;
                  };
                  float tmpvar_51;
                  tmpvar_51 = (1.0/(((_ZBufferParams.x * texture2DLod (_CameraDepthTexture, ps43testy_48.xy, 0.0).x) + _ZBufferParams.y)));
                  float tmpvar_52;
                  tmpvar_52 = (1.0/(((_ZBufferParams.x * ps43testy_48.z) + _ZBufferParams.y)));
                  if ((tmpvar_51 < tmpvar_52)) {
                    if (((tmpvar_52 - tmpvar_51) < _bias)) {
                      iotared_41.w = 1.0;
                      iotared_41.xyz = ps43testy_48;
                      ghfjghtbbv_45 = iotared_41;
                      oopplx_44 = bool(1);
                      break;
                    };
                    vec3 tmpvar_53;
                    tmpvar_53 = (freg44r_47 * 0.5);
                    freg44r_47 = tmpvar_53;
                    ps43testy_48 = (opl50op_42 + tmpvar_53);
                  } else {
                    opl50op_42 = ps43testy_48;
                    ps43testy_48 = (ps43testy_48 + freg44r_47);
                  };
                  i49opght_43 = (i49opght_43 + 1);
                  j_40 = (j_40 + 1);
                };
                if ((oopplx_44 == bool(0))) {
                  vec4 vsap55f_54;
                  vsap55f_54.w = 0.0;
                  vsap55f_54.xyz = ps43testy_48;
                  ghfjghtbbv_45 = vsap55f_54;
                  oopplx_44 = bool(1);
                };
                resfinaelxe_2 = ghfjghtbbv_45;
              };
              if ((resfinaelxe_2.w < 0.01)) {
                frefcol2d_3 = pacolac2s_8;
              } else {
                vec4 ui57tefrt_55;
                ui57tefrt_55.xyz = texture2DLod (_MainTex, resfinaelxe_2.xy, 0.0).xyz;
                ui57tefrt_55.w = (((resfinaelxe_2.w * (1.0 - (tmpvar_7 / _maxDepthCull))) * (1.0 - pow ((ql30fg_14 / float(tmpvar_23)), _fadePower))) * pow (clamp (((dot (normalize(tmpvar_28), normalize(tmpvar_25).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                frefcol2d_3 = ui57tefrt_55;
              };
            };
          };
        };
      };
    };
  };
  gl_FragData[0] = frefcol2d_3;
}


#endif
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"vs_3_0
; 5 ALU
dcl_position o0
dcl_texcoord0 o1
dcl_position0 v0
dcl_texcoord0 v1
mov o1.xy, v1
dp4 o0.w, v0, c3
dp4 o0.z, v0, c2
dp4 o0.y, v0, c1
dp4 o0.x, v0, c0
"
}

SubProgram "d3d11 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
BindCB "UnityPerDraw" 0
// 6 instructions, 1 temp regs, 0 temp arrays:
// ALU 4 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec2 xlv_TEXCOORD0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shader_texture_lod : enable
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SSRRcomposeMode;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _CameraNormalsTexture;
uniform highp mat4 _ViewMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ProjMatrix;
uniform highp float _bias;
uniform highp float _stepGlobalScale;
uniform highp float _maxStep;
uniform highp float _maxFineStep;
uniform highp float _maxDepthCull;
uniform highp float _fadePower;
uniform sampler2D _MainTex;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _ScreenParams;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 osdfej3_2;
  highp vec3 sspref3df_3;
  highp vec4 resfinaelxe_4;
  highp vec4 frefcol2d_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2DLodEXT (_MainTex, xlv_TEXCOORD0, 0.0);
  osdfej3_2 = tmpvar_6;
  if ((osdfej3_2.w == 0.0)) {
    frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
  } else {
    highp float feoimdf_7;
    lowp float tmpvar_8;
    tmpvar_8 = texture2DLodEXT (_CameraDepthTexture, xlv_TEXCOORD0, 0.0).x;
    feoimdf_7 = tmpvar_8;
    highp float tmpvar_9;
    tmpvar_9 = (1.0/(((_ZBufferParams.x * feoimdf_7) + _ZBufferParams.y)));
    if ((tmpvar_9 > _maxDepthCull)) {
      frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
      highp vec4 pacolac2s_10;
      int s_11;
      highp vec4 decrect36s_12;
      int i33tyux_13;
      bool boo32df_14;
      highp vec4 hheropg_15;
      highp float ql30fg_16;
      int mx29iujh_17;
      highp vec3 sam27pio_18;
      highp vec3 v26o9ij_19;
      highp vec3 j23kqa_20;
      highp vec4 ght43s_21;
      highp vec4 nmgghg16y_22;
      highp vec3 oy15df_23;
      highp vec4 v12fefsk_24;
      int tmpvar_25;
      tmpvar_25 = int(_maxStep);
      v12fefsk_24.w = 1.0;
      v12fefsk_24.xy = ((xlv_TEXCOORD0 * 2.0) - 1.0);
      v12fefsk_24.z = feoimdf_7;
      highp vec4 tmpvar_26;
      tmpvar_26 = (_ProjectionInv * v12fefsk_24);
      highp vec4 tmpvar_27;
      tmpvar_27 = (tmpvar_26 / tmpvar_26.w);
      oy15df_23.xy = v12fefsk_24.xy;
      oy15df_23.z = feoimdf_7;
      nmgghg16y_22.w = 0.0;
      lowp vec3 tmpvar_28;
      tmpvar_28 = ((texture2DLodEXT (_CameraNormalsTexture, xlv_TEXCOORD0, 0.0).xyz * 2.0) - 1.0);
      nmgghg16y_22.xyz = tmpvar_28;
      highp vec3 tmpvar_29;
      tmpvar_29 = normalize(tmpvar_27.xyz);
      highp vec3 tmpvar_30;
      tmpvar_30 = normalize((_ViewMatrix * nmgghg16y_22).xyz);
      highp vec3 tmpvar_31;
      tmpvar_31 = normalize((tmpvar_29 - (2.0 * (dot (tmpvar_30, tmpvar_29) * tmpvar_30))));
      ght43s_21.w = 1.0;
      ght43s_21.xyz = (tmpvar_27.xyz + tmpvar_31);
      highp vec4 tmpvar_32;
      tmpvar_32 = (_ProjMatrix * ght43s_21);
      highp vec3 tmpvar_33;
      tmpvar_33 = normalize(((tmpvar_32.xyz / tmpvar_32.w) - oy15df_23));
      sspref3df_3.z = tmpvar_33.z;
      sspref3df_3.xy = (tmpvar_33.xy * 0.5);
      j23kqa_20.xy = xlv_TEXCOORD0;
      j23kqa_20.z = feoimdf_7;
      highp float tmpvar_34;
      tmpvar_34 = (2.0 / _ScreenParams.x);
      highp float tmpvar_35;
      tmpvar_35 = sqrt(dot (sspref3df_3.xy, sspref3df_3.xy));
      highp vec3 tmpvar_36;
      tmpvar_36 = (sspref3df_3 * ((tmpvar_34 * _stepGlobalScale) / tmpvar_35));
      v26o9ij_19 = tmpvar_36;
      mx29iujh_17 = int(_maxStep);
      ql30fg_16 = 0.0;
      boo32df_14 = bool(0);
      sam27pio_18 = (j23kqa_20 + tmpvar_36);
      i33tyux_13 = 0;
      s_11 = 0;
      for (int s_11 = 0; s_11 < 99; ) {
        if ((i33tyux_13 >= mx29iujh_17)) {
          break;
        };
        lowp vec4 tmpvar_37;
        tmpvar_37 = texture2DLodEXT (_CameraDepthTexture, sam27pio_18.xy, 0.0);
        highp float tmpvar_38;
        tmpvar_38 = (1.0/(((_ZBufferParams.x * tmpvar_37.x) + _ZBufferParams.y)));
        highp float tmpvar_39;
        tmpvar_39 = (1.0/(((_ZBufferParams.x * sam27pio_18.z) + _ZBufferParams.y)));
        if ((tmpvar_38 < (tmpvar_39 - 1e-06))) {
          decrect36s_12.w = 1.0;
          decrect36s_12.xyz = sam27pio_18;
          hheropg_15 = decrect36s_12;
          boo32df_14 = bool(1);
          break;
        };
        sam27pio_18 = (sam27pio_18 + v26o9ij_19);
        ql30fg_16 = (ql30fg_16 + 1.0);
        i33tyux_13 = (i33tyux_13 + 1);
        s_11 = (s_11 + 1);
      };
      if ((boo32df_14 == bool(0))) {
        highp vec4 tpv37xs_40;
        tpv37xs_40.w = 0.0;
        tpv37xs_40.xyz = sam27pio_18;
        hheropg_15 = tpv37xs_40;
        boo32df_14 = bool(1);
      };
      resfinaelxe_4 = hheropg_15;
      highp float tmpvar_41;
      tmpvar_41 = abs((hheropg_15.x - 0.5));
      pacolac2s_10 = vec4(0.0, 0.0, 0.0, 0.0);
      if ((_SSRRcomposeMode > 0.0)) {
        highp vec4 tmpvar_42;
        tmpvar_42.w = 0.0;
        tmpvar_42.xyz = osdfej3_2.xyz;
        pacolac2s_10 = tmpvar_42;
      };
      if ((tmpvar_41 > 0.5)) {
        frefcol2d_5 = pacolac2s_10;
      } else {
        highp float tmpvar_43;
        tmpvar_43 = abs((hheropg_15.y - 0.5));
        if ((tmpvar_43 > 0.5)) {
          frefcol2d_5 = pacolac2s_10;
        } else {
          if (((1.0/(((_ZBufferParams.x * hheropg_15.z) + _ZBufferParams.y))) > _maxDepthCull)) {
            frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
          } else {
            if ((hheropg_15.z < 0.1)) {
              frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
            } else {
              if ((hheropg_15.w == 1.0)) {
                int j_44;
                highp vec4 iotared_45;
                highp vec3 opl50op_46;
                int i49opght_47;
                bool oopplx_48;
                highp vec4 ghfjghtbbv_49;
                int mxc45ui_50;
                highp vec3 freg44r_51;
                highp vec3 ps43testy_52;
                highp vec3 tmpvar_53;
                tmpvar_53 = (hheropg_15.xyz - tmpvar_36);
                highp vec3 tmpvar_54;
                tmpvar_54 = (sspref3df_3 * (tmpvar_34 / tmpvar_35));
                freg44r_51 = tmpvar_54;
                mxc45ui_50 = int(_maxFineStep);
                oopplx_48 = bool(0);
                opl50op_46 = tmpvar_53;
                ps43testy_52 = (tmpvar_53 + tmpvar_54);
                i49opght_47 = 0;
                j_44 = 0;
                for (int j_44 = 0; j_44 < 20; ) {
                  if ((i49opght_47 >= mxc45ui_50)) {
                    break;
                  };
                  lowp vec4 tmpvar_55;
                  tmpvar_55 = texture2DLodEXT (_CameraDepthTexture, ps43testy_52.xy, 0.0);
                  highp float tmpvar_56;
                  tmpvar_56 = (1.0/(((_ZBufferParams.x * tmpvar_55.x) + _ZBufferParams.y)));
                  highp float tmpvar_57;
                  tmpvar_57 = (1.0/(((_ZBufferParams.x * ps43testy_52.z) + _ZBufferParams.y)));
                  if ((tmpvar_56 < tmpvar_57)) {
                    if (((tmpvar_57 - tmpvar_56) < _bias)) {
                      iotared_45.w = 1.0;
                      iotared_45.xyz = ps43testy_52;
                      ghfjghtbbv_49 = iotared_45;
                      oopplx_48 = bool(1);
                      break;
                    };
                    highp vec3 tmpvar_58;
                    tmpvar_58 = (freg44r_51 * 0.5);
                    freg44r_51 = tmpvar_58;
                    ps43testy_52 = (opl50op_46 + tmpvar_58);
                  } else {
                    opl50op_46 = ps43testy_52;
                    ps43testy_52 = (ps43testy_52 + freg44r_51);
                  };
                  i49opght_47 = (i49opght_47 + 1);
                  j_44 = (j_44 + 1);
                };
                if ((oopplx_48 == bool(0))) {
                  highp vec4 vsap55f_59;
                  vsap55f_59.w = 0.0;
                  vsap55f_59.xyz = ps43testy_52;
                  ghfjghtbbv_49 = vsap55f_59;
                  oopplx_48 = bool(1);
                };
                resfinaelxe_4 = ghfjghtbbv_49;
              };
              if ((resfinaelxe_4.w < 0.01)) {
                frefcol2d_5 = pacolac2s_10;
              } else {
                highp vec4 ui57tefrt_60;
                lowp vec3 tmpvar_61;
                tmpvar_61 = texture2DLodEXT (_MainTex, resfinaelxe_4.xy, 0.0).xyz;
                ui57tefrt_60.xyz = tmpvar_61;
                ui57tefrt_60.w = (((resfinaelxe_4.w * (1.0 - (tmpvar_9 / _maxDepthCull))) * (1.0 - pow ((ql30fg_16 / float(tmpvar_25)), _fadePower))) * pow (clamp (((dot (normalize(tmpvar_31), normalize(tmpvar_27).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                frefcol2d_5 = ui57tefrt_60;
              };
            };
          };
        };
      };
    };
  };
  tmpvar_1 = frefcol2d_5;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec2 xlv_TEXCOORD0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shader_texture_lod : enable
varying highp vec2 xlv_TEXCOORD0;
uniform highp float _SSRRcomposeMode;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _CameraNormalsTexture;
uniform highp mat4 _ViewMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ProjMatrix;
uniform highp float _bias;
uniform highp float _stepGlobalScale;
uniform highp float _maxStep;
uniform highp float _maxFineStep;
uniform highp float _maxDepthCull;
uniform highp float _fadePower;
uniform sampler2D _MainTex;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _ScreenParams;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 osdfej3_2;
  highp vec3 sspref3df_3;
  highp vec4 resfinaelxe_4;
  highp vec4 frefcol2d_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2DLodEXT (_MainTex, xlv_TEXCOORD0, 0.0);
  osdfej3_2 = tmpvar_6;
  if ((osdfej3_2.w == 0.0)) {
    frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
  } else {
    highp float feoimdf_7;
    lowp float tmpvar_8;
    tmpvar_8 = texture2DLodEXT (_CameraDepthTexture, xlv_TEXCOORD0, 0.0).x;
    feoimdf_7 = tmpvar_8;
    highp float tmpvar_9;
    tmpvar_9 = (1.0/(((_ZBufferParams.x * feoimdf_7) + _ZBufferParams.y)));
    if ((tmpvar_9 > _maxDepthCull)) {
      frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
      highp vec4 pacolac2s_10;
      int s_11;
      highp vec4 decrect36s_12;
      int i33tyux_13;
      bool boo32df_14;
      highp vec4 hheropg_15;
      highp float ql30fg_16;
      int mx29iujh_17;
      highp vec3 sam27pio_18;
      highp vec3 v26o9ij_19;
      highp vec3 j23kqa_20;
      highp vec4 ght43s_21;
      highp vec4 nmgghg16y_22;
      highp vec3 oy15df_23;
      highp vec4 v12fefsk_24;
      int tmpvar_25;
      tmpvar_25 = int(_maxStep);
      v12fefsk_24.w = 1.0;
      v12fefsk_24.xy = ((xlv_TEXCOORD0 * 2.0) - 1.0);
      v12fefsk_24.z = feoimdf_7;
      highp vec4 tmpvar_26;
      tmpvar_26 = (_ProjectionInv * v12fefsk_24);
      highp vec4 tmpvar_27;
      tmpvar_27 = (tmpvar_26 / tmpvar_26.w);
      oy15df_23.xy = v12fefsk_24.xy;
      oy15df_23.z = feoimdf_7;
      nmgghg16y_22.w = 0.0;
      lowp vec3 tmpvar_28;
      tmpvar_28 = ((texture2DLodEXT (_CameraNormalsTexture, xlv_TEXCOORD0, 0.0).xyz * 2.0) - 1.0);
      nmgghg16y_22.xyz = tmpvar_28;
      highp vec3 tmpvar_29;
      tmpvar_29 = normalize(tmpvar_27.xyz);
      highp vec3 tmpvar_30;
      tmpvar_30 = normalize((_ViewMatrix * nmgghg16y_22).xyz);
      highp vec3 tmpvar_31;
      tmpvar_31 = normalize((tmpvar_29 - (2.0 * (dot (tmpvar_30, tmpvar_29) * tmpvar_30))));
      ght43s_21.w = 1.0;
      ght43s_21.xyz = (tmpvar_27.xyz + tmpvar_31);
      highp vec4 tmpvar_32;
      tmpvar_32 = (_ProjMatrix * ght43s_21);
      highp vec3 tmpvar_33;
      tmpvar_33 = normalize(((tmpvar_32.xyz / tmpvar_32.w) - oy15df_23));
      sspref3df_3.z = tmpvar_33.z;
      sspref3df_3.xy = (tmpvar_33.xy * 0.5);
      j23kqa_20.xy = xlv_TEXCOORD0;
      j23kqa_20.z = feoimdf_7;
      highp float tmpvar_34;
      tmpvar_34 = (2.0 / _ScreenParams.x);
      highp float tmpvar_35;
      tmpvar_35 = sqrt(dot (sspref3df_3.xy, sspref3df_3.xy));
      highp vec3 tmpvar_36;
      tmpvar_36 = (sspref3df_3 * ((tmpvar_34 * _stepGlobalScale) / tmpvar_35));
      v26o9ij_19 = tmpvar_36;
      mx29iujh_17 = int(_maxStep);
      ql30fg_16 = 0.0;
      boo32df_14 = bool(0);
      sam27pio_18 = (j23kqa_20 + tmpvar_36);
      i33tyux_13 = 0;
      s_11 = 0;
      for (int s_11 = 0; s_11 < 99; ) {
        if ((i33tyux_13 >= mx29iujh_17)) {
          break;
        };
        lowp vec4 tmpvar_37;
        tmpvar_37 = texture2DLodEXT (_CameraDepthTexture, sam27pio_18.xy, 0.0);
        highp float tmpvar_38;
        tmpvar_38 = (1.0/(((_ZBufferParams.x * tmpvar_37.x) + _ZBufferParams.y)));
        highp float tmpvar_39;
        tmpvar_39 = (1.0/(((_ZBufferParams.x * sam27pio_18.z) + _ZBufferParams.y)));
        if ((tmpvar_38 < (tmpvar_39 - 1e-06))) {
          decrect36s_12.w = 1.0;
          decrect36s_12.xyz = sam27pio_18;
          hheropg_15 = decrect36s_12;
          boo32df_14 = bool(1);
          break;
        };
        sam27pio_18 = (sam27pio_18 + v26o9ij_19);
        ql30fg_16 = (ql30fg_16 + 1.0);
        i33tyux_13 = (i33tyux_13 + 1);
        s_11 = (s_11 + 1);
      };
      if ((boo32df_14 == bool(0))) {
        highp vec4 tpv37xs_40;
        tpv37xs_40.w = 0.0;
        tpv37xs_40.xyz = sam27pio_18;
        hheropg_15 = tpv37xs_40;
        boo32df_14 = bool(1);
      };
      resfinaelxe_4 = hheropg_15;
      highp float tmpvar_41;
      tmpvar_41 = abs((hheropg_15.x - 0.5));
      pacolac2s_10 = vec4(0.0, 0.0, 0.0, 0.0);
      if ((_SSRRcomposeMode > 0.0)) {
        highp vec4 tmpvar_42;
        tmpvar_42.w = 0.0;
        tmpvar_42.xyz = osdfej3_2.xyz;
        pacolac2s_10 = tmpvar_42;
      };
      if ((tmpvar_41 > 0.5)) {
        frefcol2d_5 = pacolac2s_10;
      } else {
        highp float tmpvar_43;
        tmpvar_43 = abs((hheropg_15.y - 0.5));
        if ((tmpvar_43 > 0.5)) {
          frefcol2d_5 = pacolac2s_10;
        } else {
          if (((1.0/(((_ZBufferParams.x * hheropg_15.z) + _ZBufferParams.y))) > _maxDepthCull)) {
            frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
          } else {
            if ((hheropg_15.z < 0.1)) {
              frefcol2d_5 = vec4(0.0, 0.0, 0.0, 0.0);
            } else {
              if ((hheropg_15.w == 1.0)) {
                int j_44;
                highp vec4 iotared_45;
                highp vec3 opl50op_46;
                int i49opght_47;
                bool oopplx_48;
                highp vec4 ghfjghtbbv_49;
                int mxc45ui_50;
                highp vec3 freg44r_51;
                highp vec3 ps43testy_52;
                highp vec3 tmpvar_53;
                tmpvar_53 = (hheropg_15.xyz - tmpvar_36);
                highp vec3 tmpvar_54;
                tmpvar_54 = (sspref3df_3 * (tmpvar_34 / tmpvar_35));
                freg44r_51 = tmpvar_54;
                mxc45ui_50 = int(_maxFineStep);
                oopplx_48 = bool(0);
                opl50op_46 = tmpvar_53;
                ps43testy_52 = (tmpvar_53 + tmpvar_54);
                i49opght_47 = 0;
                j_44 = 0;
                for (int j_44 = 0; j_44 < 20; ) {
                  if ((i49opght_47 >= mxc45ui_50)) {
                    break;
                  };
                  lowp vec4 tmpvar_55;
                  tmpvar_55 = texture2DLodEXT (_CameraDepthTexture, ps43testy_52.xy, 0.0);
                  highp float tmpvar_56;
                  tmpvar_56 = (1.0/(((_ZBufferParams.x * tmpvar_55.x) + _ZBufferParams.y)));
                  highp float tmpvar_57;
                  tmpvar_57 = (1.0/(((_ZBufferParams.x * ps43testy_52.z) + _ZBufferParams.y)));
                  if ((tmpvar_56 < tmpvar_57)) {
                    if (((tmpvar_57 - tmpvar_56) < _bias)) {
                      iotared_45.w = 1.0;
                      iotared_45.xyz = ps43testy_52;
                      ghfjghtbbv_49 = iotared_45;
                      oopplx_48 = bool(1);
                      break;
                    };
                    highp vec3 tmpvar_58;
                    tmpvar_58 = (freg44r_51 * 0.5);
                    freg44r_51 = tmpvar_58;
                    ps43testy_52 = (opl50op_46 + tmpvar_58);
                  } else {
                    opl50op_46 = ps43testy_52;
                    ps43testy_52 = (ps43testy_52 + freg44r_51);
                  };
                  i49opght_47 = (i49opght_47 + 1);
                  j_44 = (j_44 + 1);
                };
                if ((oopplx_48 == bool(0))) {
                  highp vec4 vsap55f_59;
                  vsap55f_59.w = 0.0;
                  vsap55f_59.xyz = ps43testy_52;
                  ghfjghtbbv_49 = vsap55f_59;
                  oopplx_48 = bool(1);
                };
                resfinaelxe_4 = ghfjghtbbv_49;
              };
              if ((resfinaelxe_4.w < 0.01)) {
                frefcol2d_5 = pacolac2s_10;
              } else {
                highp vec4 ui57tefrt_60;
                lowp vec3 tmpvar_61;
                tmpvar_61 = texture2DLodEXT (_MainTex, resfinaelxe_4.xy, 0.0).xyz;
                ui57tefrt_60.xyz = tmpvar_61;
                ui57tefrt_60.w = (((resfinaelxe_4.w * (1.0 - (tmpvar_9 / _maxDepthCull))) * (1.0 - pow ((ql30fg_16 / float(tmpvar_25)), _fadePower))) * pow (clamp (((dot (normalize(tmpvar_31), normalize(tmpvar_27).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                frefcol2d_5 = ui57tefrt_60;
              };
            };
          };
        };
      };
    };
  };
  tmpvar_1 = frefcol2d_5;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;

#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 330
struct v2f {
    highp vec4 pos;
    highp vec2 uv;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform sampler2D _depthTexCustom;
uniform sampler2D _MainTex;
uniform highp float _fadePower;
uniform highp float _maxDepthCull;
#line 319
uniform highp float _maxFineStep;
uniform highp float _maxStep;
uniform highp float _stepGlobalScale;
uniform highp float _bias;
#line 323
uniform highp mat4 _ProjMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ViewMatrix;
uniform highp vec4 _ProjInfo;
#line 327
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
uniform highp float _SSRRcomposeMode;
#line 336
#line 336
v2f vert( in appdata_img v ) {
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    #line 340
    o.uv = v.texcoord.xy;
    return o;
}
out highp vec2 xlv_TEXCOORD0;
void main() {
    v2f xl_retval;
    appdata_img xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];
vec4 xll_tex2Dlod(sampler2D s, vec4 coord) {
   return textureLod( s, coord.xy, coord.w);
}
#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 330
struct v2f {
    highp vec4 pos;
    highp vec2 uv;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform sampler2D _depthTexCustom;
uniform sampler2D _MainTex;
uniform highp float _fadePower;
uniform highp float _maxDepthCull;
#line 319
uniform highp float _maxFineStep;
uniform highp float _maxStep;
uniform highp float _stepGlobalScale;
uniform highp float _bias;
#line 323
uniform highp mat4 _ProjMatrix;
uniform highp mat4 _ProjectionInv;
uniform highp mat4 _ViewMatrix;
uniform highp vec4 _ProjInfo;
#line 327
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
uniform highp float _SSRRcomposeMode;
#line 336
#line 343
mediump vec4 frag( in v2f i ) {
    #line 345
    highp vec4 frefcol2d;
    highp float len2ovaed;
    highp vec4 resfinaelxe;
    highp vec3 sspref3df;
    #line 349
    highp int maxcdfoief;
    highp vec4 osdfej3 = xll_tex2Dlod( _MainTex, vec4( i.uv, 0.0, 0.0));
    if ((osdfej3.w == 0.0)){
        #line 353
        frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
    }
    else{
        #line 357
        highp float feoimdf = xll_tex2Dlod( _CameraDepthTexture, vec4( i.uv, 0.0, 0.0)).x;
        highp float efopafeod2s = feoimdf;
        highp float v11dflke = (1.0 / ((_ZBufferParams.x * feoimdf) + _ZBufferParams.y));
        if ((v11dflke > _maxDepthCull)){
            #line 362
            frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
        }
        else{
            #line 366
            maxcdfoief = int(_maxStep);
            highp vec4 v12fefsk;
            v12fefsk.w = 1.0;
            v12fefsk.xy = ((i.uv * 2.0) - 1.0);
            #line 370
            v12fefsk.z = efopafeod2s;
            highp vec4 v13uujgh = (_ProjectionInv * v12fefsk);
            highp vec4 v14kkmng = (v13uujgh / v13uujgh.w);
            highp vec3 oy15df;
            #line 374
            oy15df.xy = v12fefsk.xy;
            oy15df.z = efopafeod2s;
            highp vec4 nmgghg16y;
            nmgghg16y.w = 0.0;
            #line 378
            nmgghg16y.xyz = ((xll_tex2Dlod( _CameraNormalsTexture, vec4( i.uv, 0.0, 0.0)).xyz * 2.0) - 1.0);
            highp vec3 uuyq32d = normalize(v14kkmng.xyz);
            highp vec3 f18iop = normalize((_ViewMatrix * nmgghg16y).xyz);
            highp vec3 v19vvdss = normalize((uuyq32d - (2.0 * (dot( f18iop, uuyq32d) * f18iop))));
            #line 382
            highp vec4 ght43s;
            ght43s.w = 1.0;
            ght43s.xyz = (v14kkmng.xyz + v19vvdss);
            highp vec4 retdfsqs = (_ProjMatrix * ght43s);
            #line 386
            highp vec3 t22ucvf = normalize(((retdfsqs.xyz / retdfsqs.w) - oy15df));
            sspref3df.z = t22ucvf.z;
            sspref3df.xy = (t22ucvf.xy * 0.5);
            highp vec3 j23kqa;
            #line 390
            j23kqa.xy = i.uv;
            j23kqa.z = efopafeod2s;
            len2ovaed = 0.0;
            highp float bberesa = (2.0 / _ScreenParams.x);
            #line 394
            highp float trdfgr25t = sqrt(dot( sspref3df.xy, sspref3df.xy));
            highp vec3 v26o9ij = (sspref3df * ((bberesa * _stepGlobalScale) / trdfgr25t));
            highp vec3 sam27pio;
            highp int mx29iujh = int(_maxStep);
            #line 398
            highp float ql30fg = len2ovaed;
            highp vec4 hheropg;
            bool boo32df = false;
            sam27pio = (j23kqa + v26o9ij);
            #line 402
            highp int i33tyux = 0;
            highp float tpv34gsf;
            highp float vrtoinhx;
            highp vec4 decrect36s;
            #line 406
            highp int s = 0;
            s = 0;
            for ( ; (s < 99); (s++)) {
                #line 411
                if ((i33tyux >= mx29iujh)){
                    break;
                }
                #line 415
                tpv34gsf = (1.0 / ((_ZBufferParams.x * xll_tex2Dlod( _CameraDepthTexture, vec4( sam27pio.xy, 0.0, 0.0)).x) + _ZBufferParams.y));
                vrtoinhx = (1.0 / ((_ZBufferParams.x * sam27pio.z) + _ZBufferParams.y));
                if ((tpv34gsf < (vrtoinhx - 1e-06))){
                    #line 419
                    decrect36s.w = 1.0;
                    decrect36s.xyz = sam27pio;
                    hheropg = decrect36s;
                    boo32df = true;
                    #line 423
                    break;
                }
                sam27pio = (sam27pio + v26o9ij);
                ql30fg = (ql30fg + 1.0);
                #line 427
                i33tyux = (i33tyux + 1);
            }
            if ((boo32df == false)){
                #line 431
                highp vec4 tpv37xs;
                tpv37xs.w = 0.0;
                tpv37xs.xyz = sam27pio;
                hheropg = tpv37xs;
                #line 435
                boo32df = true;
            }
            len2ovaed = ql30fg;
            resfinaelxe = hheropg;
            #line 439
            highp float tpv38xoi;
            tpv38xoi = abs((hheropg.x - 0.5));
            highp vec4 pacolac2s = vec4( 0.0, 0.0, 0.0, 0.0);
            if ((_SSRRcomposeMode > 0.0)){
                pacolac2s = vec4( osdfej3.xyz, 0.0);
            }
            #line 443
            if ((tpv38xoi > 0.5)){
                frefcol2d = pacolac2s;
            }
            else{
                #line 449
                highp float op39xcv = abs((hheropg.y - 0.5));
                if ((op39xcv > 0.5)){
                    frefcol2d = pacolac2s;
                }
                else{
                    #line 456
                    if (((1.0 / ((_ZBufferParams.x * hheropg.z) + _ZBufferParams.y)) > _maxDepthCull)){
                        frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
                    }
                    else{
                        #line 462
                        if ((hheropg.z < 0.1)){
                            frefcol2d = vec4( 0.0, 0.0, 0.0, 0.0);
                        }
                        else{
                            #line 468
                            if ((hheropg.w == 1.0)){
                                highp vec3 v41yup = (hheropg.xyz - v26o9ij);
                                highp vec3 yipr42x = (sspref3df * (bberesa / trdfgr25t));
                                #line 472
                                highp vec3 ps43testy;
                                highp vec3 freg44r;
                                freg44r = yipr42x;
                                highp int mxc45ui = int(_maxFineStep);
                                #line 476
                                highp vec4 ghfjghtbbv;
                                bool oopplx = false;
                                highp int i49opght;
                                highp vec3 opl50op = v41yup;
                                #line 480
                                ps43testy = (v41yup + yipr42x);
                                i49opght = 0;
                                highp float vre51lv;
                                highp float lkde52xw;
                                #line 484
                                highp vec4 iotared;
                                highp vec3 yrrkjgf54t;
                                highp int j = 0;
                                j = 0;
                                for ( ; (j < 20); (j++)) {
                                    #line 491
                                    if ((i49opght >= mxc45ui)){
                                        break;
                                    }
                                    #line 495
                                    vre51lv = (1.0 / ((_ZBufferParams.x * xll_tex2Dlod( _CameraDepthTexture, vec4( ps43testy.xy, 0.0, 0.0)).x) + _ZBufferParams.y));
                                    lkde52xw = (1.0 / ((_ZBufferParams.x * ps43testy.z) + _ZBufferParams.y));
                                    if ((vre51lv < lkde52xw)){
                                        #line 499
                                        if (((lkde52xw - vre51lv) < _bias)){
                                            iotared.w = 1.0;
                                            iotared.xyz = ps43testy;
                                            #line 503
                                            ghfjghtbbv = iotared;
                                            oopplx = true;
                                            break;
                                        }
                                        #line 507
                                        yrrkjgf54t = (freg44r * 0.5);
                                        freg44r = yrrkjgf54t;
                                        ps43testy = (opl50op + yrrkjgf54t);
                                    }
                                    else{
                                        #line 513
                                        opl50op = ps43testy;
                                        ps43testy = (ps43testy + freg44r);
                                    }
                                    i49opght = (i49opght + 1);
                                }
                                #line 518
                                if ((oopplx == false)){
                                    highp vec4 vsap55f;
                                    vsap55f.w = 0.0;
                                    #line 522
                                    vsap55f.xyz = ps43testy;
                                    ghfjghtbbv = vsap55f;
                                    oopplx = true;
                                }
                                #line 526
                                resfinaelxe = ghfjghtbbv;
                            }
                            if ((resfinaelxe.w < 0.01)){
                                #line 530
                                frefcol2d = pacolac2s;
                            }
                            else{
                                #line 534
                                highp vec4 ui57tefrt;
                                ui57tefrt.xyz = xll_tex2Dlod( _MainTex, vec4( resfinaelxe.xy, 0.0, 0.0)).xyz;
                                ui57tefrt.w = (((resfinaelxe.w * (1.0 - (v11dflke / _maxDepthCull))) * (1.0 - pow( (ql30fg / float(maxcdfoief)), _fadePower))) * pow( clamp( ((dot( normalize(v19vvdss), normalize(v14kkmng).xyz) + 1.0) + (_fadePower * 0.1)), 0.0, 1.0), _fadePower));
                                frefcol2d = ui57tefrt;
                            }
                        }
                    }
                }
            }
        }
    }
    #line 545
    return frefcol2d;
}
in highp vec2 xlv_TEXCOORD0;
void main() {
    mediump vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   d3d9 - ALU: 176 to 176, TEX: 12 to 12, FLOW: 25 to 25
//   d3d11 - ALU: 107 to 107, TEX: 0 to 0, FLOW: 29 to 29
SubProgram "opengl " {
Keywords { }
"!!GLSL"
}

SubProgram "d3d9 " {
Keywords { }
Vector 12 [_ScreenParams]
Vector 13 [_ZBufferParams]
Float 14 [_fadePower]
Float 15 [_maxDepthCull]
Float 16 [_maxFineStep]
Float 17 [_maxStep]
Float 18 [_stepGlobalScale]
Float 19 [_bias]
Matrix 0 [_ProjMatrix]
Matrix 4 [_ProjectionInv]
Matrix 8 [_ViewMatrix]
Float 20 [_SSRRcomposeMode]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_CameraDepthTexture] 2D
SetTexture 2 [_CameraNormalsTexture] 2D
"ps_3_0
; 176 ALU, 12 TEX, 25 FLOW
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c21, 0.00000000, 2.00000000, -1.00000000, 1.00000000
def c22, 0.50000000, 1.00000000, -0.00000100, -0.50000000
defi i0, 99, 0, 1, 0
def c23, 0.10000000, 0.01000000, 0, 0
defi i1, 20, 0, 1, 0
dcl_texcoord0 v0.xy
mov r0.xy, v0
mov r0.z, c21.x
texldl r3, r0.xyzz, s0
if_eq r3.w, c21.x
mov r0, c21.x
else
mov r0.xy, v0
mov r0.z, c21.x
texldl r0.x, r0.xyzz, s1
mad r0.y, r0.x, c13.x, c13
rcp r4.w, r0.y
if_gt r4.w, c15.x
mov r0, c21.x
else
mad r8.xy, v0, c21.y, c21.z
mov r5.z, r0.x
mov r5.xy, r8
mov r5.w, c21
dp4 r0.y, r5, c7
mov r1.w, r0.y
mov r6.w, c21
dp4 r1.z, r5, c6
dp4 r1.y, r5, c5
dp4 r1.x, r5, c4
rcp r0.y, r0.y
mul r1, r1, r0.y
dp3 r0.z, r1, r1
mov r5.w, c21.x
mov r8.z, r0.x
rsq r0.z, r0.z
mov r5.z, c21.x
mov r5.xy, v0
texldl r5.xyz, r5.xyzz, s2
mad r5.xyz, r5, c21.y, c21.z
dp4 r6.z, r5, c10
dp4 r6.x, r5, c8
dp4 r6.y, r5, c9
dp3 r0.y, r6, r6
rsq r0.y, r0.y
mul r5.xyz, r0.z, r1
mul r6.xyz, r0.y, r6
dp3 r0.y, r6, r5
mul r6.xyz, r0.y, r6
mad r5.xyz, -r6, c21.y, r5
dp3 r0.y, r5, r5
rsq r0.y, r0.y
mul r5.xyz, r0.y, r5
add r6.xyz, r1, r5
dp4 r0.y, r6, c3
dp4 r7.z, r6, c2
dp4 r7.y, r6, c1
dp4 r7.x, r6, c0
rcp r0.y, r0.y
mad r6.xyz, r7, r0.y, -r8
dp3 r0.y, r6, r6
rsq r0.y, r0.y
mul r6.xyz, r0.y, r6
mul r0.zw, r6.xyxy, c22.x
rcp r0.y, c12.x
mul r0.zw, r0, r0
mul r7.w, r0.y, c21.y
add r0.y, r0.z, r0.w
rsq r5.w, r0.y
mul r0.z, r7.w, c18.x
mul r0.y, r5.w, r0.z
abs r0.w, c17.x
frc r3.w, r0
add r0.w, r0, -r3
mul r6.xyz, r6, c22.xxyw
mul r7.xyz, r6, r0.y
mov r0.z, r0.x
mov r0.xy, v0
add r8.xyz, r7, r0
cmp r0.x, c17, r0.w, -r0.w
rcp r8.w, r5.w
mov r5.w, r0.x
mov r9.x, r0
mov r6.w, c21.x
mov_pp r3.w, c21.x
mov r9.y, c21.x
loop aL, i0
break_ge r9.y, r9.x
mad r0.w, r8.z, c13.x, c13.y
mov r0.z, c21.x
mov r0.xy, r8
texldl r0.x, r0.xyzz, s1
rcp r0.y, r0.w
mad r0.x, r0, c13, c13.y
add r9.w, r0.y, c22.z
rcp r9.z, r0.x
add r10.x, r9.z, -r9.w
mov r0.xyz, r8
mov r0.w, c21
cmp r2, r10.x, r2, r0
cmp_pp r3.w, r10.x, r3, c21
break_lt r9.z, r9.w
add r8.xyz, r8, r7
add r6.w, r6, c21
add r9.y, r9, c21.w
endloop
mov r0.xyz, r8
abs_pp r3.w, r3
mov r0.w, c21.x
cmp r0, -r3.w, r0, r2
add r3.w, r0.x, c22
mov r2, r0
mov r0.xyz, r0.xyww
abs r8.x, r3.w
mov r3.w, c21.x
mov r0.w, c21.x
cmp r3, -c20.x, r0.w, r3
if_gt r8.x, c22.x
mov r0, r3
else
add r0.w, r2.y, c22
abs r0.w, r0
if_gt r0.w, c22.x
mov r0, r3
else
mad r0.w, r2.z, c13.x, c13.y
rcp r0.w, r0.w
if_gt r0.w, c15.x
mov r0, c21.x
else
if_lt r2.z, c23.x
mov r0, c21.x
else
if_eq r2.w, c21.w
abs r0.y, c16.x
rcp r0.x, r8.w
mul r0.x, r7.w, r0
mul r6.xyz, r6, r0.x
add r2.xyz, r2, -r7
frc r0.z, r0.y
add r0.x, r0.y, -r0.z
add r7.xyz, r6, r2
cmp r2.w, c16.x, r0.x, -r0.x
mov_pp r0.w, c21.x
mov r7.w, c21.x
loop aL, i1
break_ge r7.w, r2.w
mov r0.z, c21.x
mov r0.xy, r7
texldl r0.x, r0.xyzz, s1
mad r0.y, r7.z, c13.x, c13
mad r0.x, r0, c13, c13.y
rcp r0.y, r0.y
rcp r0.x, r0.x
add r0.z, -r0.x, r0.y
add r0.x, r0, -r0.y
add r0.z, r0, -c19.x
cmp r0.y, r0.z, c21.x, c21.w
cmp r8.w, r0.x, c21.x, c21
mul_pp r8.x, r8.w, r0.y
mov r0.xy, r7
mov r0.z, c21.w
cmp r4.xyz, -r8.x, r4, r0
cmp_pp r0.w, -r8.x, r0, c21
break_gt r8.x, c21.x
mul r0.xyz, r6, c22.x
add r8.xyz, r0, r2
cmp r8.xyz, -r8.w, r7, r8
cmp r6.xyz, -r8.w, r6, r0
abs_pp r8.w, r8
add r0.xyz, r6, r8
cmp r7.xyz, -r8.w, r0, r8
cmp r2.xyz, -r8.w, r8, r2
add r7.w, r7, c21
endloop
mov r0.xy, r7
mov r0.z, c21.x
abs_pp r0.w, r0
cmp r0.xyz, -r0.w, r0, r4
endif
if_lt r0.z, c23.y
mov r0, r3
else
dp4 r1.w, r1, r1
rsq r1.w, r1.w
dp3 r0.w, r5, r5
mul r2.xyz, r1.w, r1
rsq r0.w, r0.w
mul r1.xyz, r0.w, r5
dp3 r1.x, r1, r2
mov r0.w, c14.x
mad r0.w, c23.x, r0, r1.x
add_sat r2.x, r0.w, c21.w
pow r1, r2.x, c14.x
rcp r0.w, r5.w
mul r0.w, r6, r0
pow r2, r0.w, c14.x
rcp r0.w, c15.x
mad r0.w, -r4, r0, c21
mul r0.z, r0, r0.w
mov r1.y, r1.x
mov r1.x, r2
add r1.x, -r1, c21.w
mul r0.z, r0, r1.x
mul r0.w, r0.z, r1.y
mov r0.z, c21.x
texldl r0.xyz, r0.xyzz, s0
endif
endif
endif
endif
endif
endif
endif
mov_pp oC0, r0
"
}

SubProgram "d3d11 " {
Keywords { }
ConstBuffer "$Globals" 272 // 260 used size, 12 vars
Float 16 [_fadePower]
Float 20 [_maxDepthCull]
Float 24 [_maxFineStep]
Float 28 [_maxStep]
Float 32 [_stepGlobalScale]
Float 36 [_bias]
Matrix 48 [_ProjMatrix] 4
Matrix 112 [_ProjectionInv] 4
Matrix 176 [_ViewMatrix] 4
Float 256 [_SSRRcomposeMode]
ConstBuffer "UnityPerCamera" 128 // 128 used size, 8 vars
Vector 96 [_ScreenParams] 4
Vector 112 [_ZBufferParams] 4
BindCB "$Globals" 0
BindCB "UnityPerCamera" 1
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 2
SetTexture 2 [_CameraNormalsTexture] 2D 1
// 201 instructions, 13 temp regs, 0 temp arrays:
// ALU 98 float, 8 int, 1 uint
// TEX 0 (6 load, 0 comp, 0 bias, 0 grad)
// FLOW 14 static, 15 dynamic
"ps_4_0
eefiecedihkiajekdgojjpdpcdmbfdlopifjgpbcabaaaaaapibeaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcdibeaaaa
eaaaaaaaaoafaaaafjaaaaaeegiocaaaaaaaaaaabbaaaaaafjaaaaaeegiocaaa
abaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaae
aahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacanaaaaaaeiaaaaal
pcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
abeaaaaaaaaaaaaabiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaa
aaaaaaaabpaaaeaddkaabaaaaaaaaaaadgaaaaaipccabaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabcaaaaabeiaaaaalpcaabaaaabaaaaaa
egbabaaaabaaaaaajghmbaaaabaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaa
dcaaaaalicaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaackaabaaaabaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpdkaabaaaaaaaaaaadbaaaaaiicaabaaaabaaaaaa
bkiacaaaaaaaaaaaabaaaaaadkaabaaaaaaaaaaabpaaaeaddkaabaaaabaaaaaa
dgaaaaaipccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
bcaaaaabdcaaaaapdcaabaaaacaaaaaaegbabaaaabaaaaaaaceaaaaaaaaaaaea
aaaaaaeaaaaaaaaaaaaaaaaaaceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaa
diaaaaaipcaabaaaadaaaaaafgafbaaaacaaaaaaegiocaaaaaaaaaaaaiaaaaaa
dcaaaaakpcaabaaaacaaaaaaegiocaaaaaaaaaaaahaaaaaaagaabaaaacaaaaaa
egaobaaaadaaaaaadcaaaaakpcaabaaaacaaaaaaegiocaaaaaaaaaaaajaaaaaa
kgakbaaaabaaaaaaegaobaaaacaaaaaaaaaaaaaipcaabaaaacaaaaaaegaobaaa
acaaaaaaegiocaaaaaaaaaaaakaaaaaaaoaaaaahpcaabaaaacaaaaaaegaobaaa
acaaaaaapgapbaaaacaaaaaaeiaaaaalpcaabaaaadaaaaaaegbabaaaabaaaaaa
eghobaaaacaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadcaaaaaphcaabaaa
adaaaaaaegacbaaaadaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaa
aceaaaaaaaaaialpaaaaialpaaaaialpaaaaaaaabaaaaaahicaabaaaabaaaaaa
egacbaaaacaaaaaaegacbaaaacaaaaaaeeaaaaaficaabaaaabaaaaaadkaabaaa
abaaaaaadiaaaaahhcaabaaaaeaaaaaapgapbaaaabaaaaaaegacbaaaacaaaaaa
diaaaaaihcaabaaaafaaaaaafgafbaaaadaaaaaaegiccaaaaaaaaaaaamaaaaaa
dcaaaaaklcaabaaaadaaaaaaegiicaaaaaaaaaaaalaaaaaaagaabaaaadaaaaaa
egaibaaaafaaaaaadcaaaaakhcaabaaaadaaaaaaegiccaaaaaaaaaaaanaaaaaa
kgakbaaaadaaaaaaegadbaaaadaaaaaabaaaaaahicaabaaaabaaaaaaegacbaaa
adaaaaaaegacbaaaadaaaaaaeeaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaa
diaaaaahhcaabaaaadaaaaaapgapbaaaabaaaaaaegacbaaaadaaaaaabaaaaaah
icaabaaaabaaaaaaegacbaaaadaaaaaaegacbaaaaeaaaaaadiaaaaahhcaabaaa
adaaaaaaegacbaaaadaaaaaapgapbaaaabaaaaaadcaaaaanhcaabaaaadaaaaaa
egacbaiaebaaaaaaadaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaa
egacbaaaaeaaaaaabaaaaaahicaabaaaabaaaaaaegacbaaaadaaaaaaegacbaaa
adaaaaaaeeaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaadiaaaaahhcaabaaa
aeaaaaaapgapbaaaabaaaaaaegacbaaaadaaaaaadcaaaaajhcaabaaaadaaaaaa
egacbaaaadaaaaaapgapbaaaabaaaaaaegacbaaaacaaaaaadiaaaaaipcaabaaa
afaaaaaafgafbaaaadaaaaaaegiocaaaaaaaaaaaaeaaaaaadcaaaaakpcaabaaa
afaaaaaaegiocaaaaaaaaaaaadaaaaaaagaabaaaadaaaaaaegaobaaaafaaaaaa
dcaaaaakpcaabaaaadaaaaaaegiocaaaaaaaaaaaafaaaaaakgakbaaaadaaaaaa
egaobaaaafaaaaaaaaaaaaaipcaabaaaadaaaaaaegaobaaaadaaaaaaegiocaaa
aaaaaaaaagaaaaaaaoaaaaahhcaabaaaadaaaaaaegacbaaaadaaaaaapgapbaaa
adaaaaaadcaaaaapdcaabaaaabaaaaaaegbabaaaabaaaaaaaceaaaaaaaaaaaea
aaaaaaeaaaaaaaaaaaaaaaaaaceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaa
aaaaaaaihcaabaaaadaaaaaaegacbaiaebaaaaaaabaaaaaaegacbaaaadaaaaaa
baaaaaahicaabaaaabaaaaaaegacbaaaadaaaaaaegacbaaaadaaaaaaeeaaaaaf
icaabaaaabaaaaaadkaabaaaabaaaaaadiaaaaahhcaabaaaadaaaaaapgapbaaa
abaaaaaaegacbaaaadaaaaaadiaaaaakdcaabaaaafaaaaaaegaabaaaadaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaaaoaaaaaiicaabaaaabaaaaaa
abeaaaaaaaaaaaeaakiacaaaabaaaaaaagaaaaaaapaaaaahicaabaaaadaaaaaa
egaabaaaafaaaaaaegaabaaaafaaaaaaelaaaaaficaabaaaadaaaaaadkaabaaa
adaaaaaadiaaaaaiicaabaaaaeaaaaaadkaabaaaabaaaaaaakiacaaaaaaaaaaa
acaaaaaaaoaaaaahicaabaaaaeaaaaaadkaabaaaaeaaaaaadkaabaaaadaaaaaa
diaaaaakhcaabaaaadaaaaaaegacbaaaadaaaaaaaceaaaaaaaaaaadpaaaaaadp
aaaaiadpaaaaaaaablaaaaagbcaabaaaafaaaaaadkiacaaaaaaaaaaaabaaaaaa
dgaaaaafdcaabaaaabaaaaaaegbabaaaabaaaaaadcaaaaajhcaabaaaabaaaaaa
egacbaaaadaaaaaapgapbaaaaeaaaaaaegacbaaaabaaaaaadgaaaaaficaabaaa
agaaaaaaabeaaaaaaaaaiadpdgaaaaaipcaabaaaahaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadgaaaaafhcaabaaaagaaaaaaegacbaaaabaaaaaa
dgaaaaaiocaabaaaafaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
dgaaaaafbcaabaaaaiaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaahccaabaaa
aiaaaaaaakaabaaaaiaaaaaaabeaaaaagdaaaaaaadaaaeadbkaabaaaaiaaaaaa
cbaaaaahccaabaaaaiaaaaaadkaabaaaafaaaaaaakaabaaaafaaaaaabpaaaead
bkaabaaaaiaaaaaaacaaaaabbfaaaaabeiaaaaalpcaabaaaajaaaaaaegaabaaa
agaaaaaaeghobaaaabaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaadcaaaaal
ccaabaaaaiaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaajaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakccaabaaaaiaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpbkaabaaaaiaaaaaadcaaaaalecaabaaaaiaaaaaaakiacaaa
abaaaaaaahaaaaaackaabaaaagaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaak
ecaabaaaaiaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpckaabaaa
aiaaaaaaaaaaaaahecaabaaaaiaaaaaackaabaaaaiaaaaaaabeaaaaalndhiglf
dbaaaaahccaabaaaaiaaaaaabkaabaaaaiaaaaaackaabaaaaiaaaaaabpaaaead
bkaabaaaaiaaaaaadgaaaaafpcaabaaaahaaaaaaegaobaaaagaaaaaadgaaaaaf
ecaabaaaafaaaaaaabeaaaaappppppppacaaaaabbfaaaaabdcaaaaajhcaabaaa
agaaaaaaegacbaaaadaaaaaapgapbaaaaeaaaaaaegacbaaaagaaaaaaaaaaaaah
ccaabaaaafaaaaaabkaabaaaafaaaaaaabeaaaaaaaaaiadpboaaaaahicaabaaa
afaaaaaadkaabaaaafaaaaaaabeaaaaaabaaaaaaboaaaaahbcaabaaaaiaaaaaa
akaabaaaaiaaaaaaabeaaaaaabaaaaaadgaaaaaipcaabaaaahaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaafecaabaaaafaaaaaaabeaaaaa
aaaaaaaabgaaaaabdgaaaaaficaabaaaagaaaaaaabeaaaaaaaaaaaaadhaaaaaj
pcaabaaaagaaaaaakgakbaaaafaaaaaaegaobaaaahaaaaaaegaobaaaagaaaaaa
aaaaaaahbcaabaaaabaaaaaaakaabaaaagaaaaaaabeaaaaaaaaaaalpdbaaaaai
ccaabaaaabaaaaaaabeaaaaaaaaaaaaaakiacaaaaaaaaaaabaaaaaaaabaaaaah
hcaabaaaahaaaaaaegacbaaaaaaaaaaafgafbaaaabaaaaaadgaaaaaficaabaaa
ahaaaaaaabeaaaaaaaaaaaaadbaaaaaibcaabaaaaaaaaaaaabeaaaaaaaaaaadp
akaabaiaibaaaaaaabaaaaaabpaaaeadakaabaaaaaaaaaaadgaaaaafpccabaaa
aaaaaaaaegaobaaaahaaaaaabcaaaaabaaaaaaahbcaabaaaaaaaaaaabkaabaaa
agaaaaaaabeaaaaaaaaaaalpdbaaaaaibcaabaaaaaaaaaaaabeaaaaaaaaaaadp
akaabaiaibaaaaaaaaaaaaaabpaaaeadakaabaaaaaaaaaaadgaaaaafpccabaaa
aaaaaaaaegaobaaaahaaaaaabcaaaaabdcaaaaalbcaabaaaaaaaaaaaakiacaaa
abaaaaaaahaaaaaackaabaaaagaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaak
bcaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpakaabaaa
aaaaaaaadbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaabaaaaaaakaabaaa
aaaaaaaabpaaaeadakaabaaaaaaaaaaadgaaaaaipccabaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabcaaaaabdbaaaaahbcaabaaaaaaaaaaa
ckaabaaaagaaaaaaabeaaaaamnmmmmdnbpaaaeadakaabaaaaaaaaaaadgaaaaai
pccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabcaaaaab
biaaaaahbcaabaaaaaaaaaaadkaabaaaagaaaaaaabeaaaaaaaaaiadpbpaaaead
akaabaaaaaaaaaaadcaaaaakhcaabaaaaaaaaaaaegacbaiaebaaaaaaadaaaaaa
pgapbaaaaeaaaaaaegacbaaaagaaaaaaaoaaaaahbcaabaaaabaaaaaadkaabaaa
abaaaaaadkaabaaaadaaaaaadiaaaaahocaabaaaabaaaaaaagaabaaaabaaaaaa
agajbaaaadaaaaaablaaaaagicaabaaaadaaaaaackiacaaaaaaaaaaaabaaaaaa
dcaaaaajhcaabaaaadaaaaaaegacbaaaadaaaaaaagaabaaaabaaaaaaegacbaaa
aaaaaaaadgaaaaafecaabaaaaiaaaaaaabeaaaaaaaaaiadpdgaaaaafncaabaaa
afaaaaaafgaobaaaabaaaaaadgaaaaaihcaabaaaajaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadgaaaaafhcaabaaaakaaaaaaegacbaaaaaaaaaaa
dgaaaaafhcaabaaaalaaaaaaegacbaaaadaaaaaadgaaaaafbcaabaaaabaaaaaa
abeaaaaaaaaaaaaadgaaaaaficaabaaaaeaaaaaaabeaaaaaaaaaaaaadgaaaaaf
icaabaaaagaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaahicaabaaaaiaaaaaa
dkaabaaaagaaaaaaabeaaaaabeaaaaaaadaaaeaddkaabaaaaiaaaaaacbaaaaah
icaabaaaaiaaaaaadkaabaaaaeaaaaaadkaabaaaadaaaaaabpaaaeaddkaabaaa
aiaaaaaaacaaaaabbfaaaaabeiaaaaalpcaabaaaamaaaaaaegaabaaaalaaaaaa
eghobaaaabaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaadcaaaaalicaabaaa
aiaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaamaaaaaabkiacaaaabaaaaaa
ahaaaaaaaoaaaaakicaabaaaaiaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpdkaabaaaaiaaaaaadcaaaaalicaabaaaajaaaaaaakiacaaaabaaaaaa
ahaaaaaackaabaaaalaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaa
ajaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaajaaaaaa
dbaaaaahicaabaaaakaaaaaadkaabaaaaiaaaaaadkaabaaaajaaaaaabpaaaead
dkaabaaaakaaaaaaaaaaaaaiicaabaaaaiaaaaaadkaabaiaebaaaaaaaiaaaaaa
dkaabaaaajaaaaaadbaaaaaiicaabaaaaiaaaaaadkaabaaaaiaaaaaabkiacaaa
aaaaaaaaacaaaaaabpaaaeaddkaabaaaaiaaaaaadgaaaaafdcaabaaaaiaaaaaa
egaabaaaalaaaaaadgaaaaafhcaabaaaajaaaaaaegacbaaaaiaaaaaadgaaaaaf
bcaabaaaabaaaaaaabeaaaaappppppppacaaaaabbfaaaaabdiaaaaaklcaabaaa
aiaaaaaaigambaaaafaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaadp
dcaaaaamhcaabaaaalaaaaaaigadbaaaafaaaaaaaceaaaaaaaaaaadpaaaaaadp
aaaaaadpaaaaaaaaegacbaaaakaaaaaadgaaaaafncaabaaaafaaaaaaaganbaaa
aiaaaaaabcaaaaabaaaaaaahlcaabaaaaiaaaaaaigambaaaafaaaaaaegaibaaa
alaaaaaadgaaaaafhcaabaaaakaaaaaaegacbaaaalaaaaaadgaaaaafhcaabaaa
alaaaaaaegadbaaaaiaaaaaabfaaaaabboaaaaahicaabaaaaeaaaaaadkaabaaa
aeaaaaaaabeaaaaaabaaaaaaboaaaaahicaabaaaagaaaaaadkaabaaaagaaaaaa
abeaaaaaabaaaaaadgaaaaaihcaabaaaajaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaadgaaaaafbcaabaaaabaaaaaaabeaaaaaaaaaaaaabgaaaaab
dgaaaaafecaabaaaalaaaaaaabeaaaaaaaaaaaaadhaaaaajhcaabaaaagaaaaaa
agaabaaaabaaaaaaegacbaaaajaaaaaaegacbaaaalaaaaaabcaaaaabdgaaaaaf
ecaabaaaagaaaaaaabeaaaaaaaaaaaaabfaaaaabdbaaaaahbcaabaaaaaaaaaaa
ckaabaaaagaaaaaaabeaaaaaaknhcddmbpaaaeadakaabaaaaaaaaaaadgaaaaaf
pccabaaaaaaaaaaaegaobaaaahaaaaaabcaaaaabeiaaaaalpcaabaaaabaaaaaa
egaabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
aoaaaaaibcaabaaaaaaaaaaadkaabaaaaaaaaaaabkiacaaaaaaaaaaaabaaaaaa
aaaaaaaibcaabaaaaaaaaaaaakaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadp
diaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaackaabaaaagaaaaaaedaaaaag
ccaabaaaaaaaaaaadkiacaaaaaaaaaaaabaaaaaaaoaaaaahccaabaaaaaaaaaaa
bkaabaaaafaaaaaabkaabaaaaaaaaaaacpaaaaafccaabaaaaaaaaaaabkaabaaa
aaaaaaaadiaaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaaakiacaaaaaaaaaaa
abaaaaaabjaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaaaaaaaaaiccaabaaa
aaaaaaaabkaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaa
aaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaabbaaaaahccaabaaaaaaaaaaa
egaobaaaacaaaaaaegaobaaaacaaaaaaeeaaaaafccaabaaaaaaaaaaabkaabaaa
aaaaaaaadiaaaaahocaabaaaaaaaaaaafgafbaaaaaaaaaaaagajbaaaacaaaaaa
baaaaaahccaabaaaaaaaaaaaegacbaaaaeaaaaaajgahbaaaaaaaaaaaaaaaaaah
ccaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaaaaaaiadpdccaaaakccaabaaa
aaaaaaaaakiacaaaaaaaaaaaabaaaaaaabeaaaaamnmmmmdnbkaabaaaaaaaaaaa
cpaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaadiaaaaaiccaabaaaaaaaaaaa
bkaabaaaaaaaaaaaakiacaaaaaaaaaaaabaaaaaabjaaaaafccaabaaaaaaaaaaa
bkaabaaaaaaaaaaadiaaaaahiccabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaa
aaaaaaaadgaaaaafhccabaaaaaaaaaaaegacbaaaabaaaaaabfaaaaabbfaaaaab
bfaaaaabbfaaaaabbfaaaaabbfaaaaabbfaaaaabdoaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 636

	}
	
	//================================================================================================================================================
	//END OF PASS 2
	//================================================================================================================================================
	//#################################################################################################################################################
	
	
}

Fallback off

}