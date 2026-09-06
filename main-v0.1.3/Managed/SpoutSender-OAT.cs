// KlakSpout - Spout realtime video sharing plugin for Unity
// https://github.com/keijiro/KlakSpout
using UnityEngine;
using OnAirTap.Klak.Spout;
using System;

namespace OnAirTap.Spout
{
    /// Spout sender class
    [AddComponentMenu("Klak/Spout/Spout Sender")]
    [ExecuteInEditMode]
    public class SpoutSender
    {
        #region Editable properties

        Texture _sourceTex;

        public Texture sourceTexture
        {
            get { return _sourceTex; }
            set { _sourceTex = value; }
        }

        Material _mat;

        public Material material
        {
            get { return _mat; }
            set { _mat = value; }
        }

        bool _vflip;

        public bool verticalFlip
        {
            get { return _vflip; }
            set { _vflip = value; }
        }

        string _spoutName = ("OATKlak" + Guid.NewGuid().ToString()).Substring(0, 30);

        public string spoutName
        {
            get { return _spoutName; }
            set { _spoutName = value; DestroyTextureAndSender(); }
        }


        #endregion

        #region Private members

        System.IntPtr _sender;
        Texture2D _sharedTexture;
        Material _blitMat;

        #endregion

        #region MonoBehaviour functions

        bool _enabled = true;

        public bool enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        void Update()
        {
            PluginEntry.Poll();
        }

        #endregion

        void DestroyTextureAndSender()
        {
            if (_sender != System.IntPtr.Zero)
            {
                PluginEntry.DestroySharedObject(_sender);
                _sender = System.IntPtr.Zero;
            }

            if (_sharedTexture != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(_sharedTexture);
                else
                    UnityEngine.Object.DestroyImmediate(_sharedTexture);
                _sharedTexture = null;
            }
        }

        void InitSender()
        {
            _sender = PluginEntry.CreateSender(_spoutName, _sourceTex.width, _sourceTex.height);
        }

        void InitSharedTexture()
        {
            if (_sender == System.IntPtr.Zero) { InitSender(); }

            if (_sender != System.IntPtr.Zero)
            {

                var ptr = PluginEntry.GetTexturePointer(_sender);
                if (ptr != System.IntPtr.Zero)
                {
                    _sharedTexture = Texture2D.CreateExternalTexture(
                        PluginEntry.GetTextureWidth(_sender),
                        PluginEntry.GetTextureHeight(_sender),
                        TextureFormat.ARGB32, false, false, ptr
                    );
                }
            }
            Debug.Log("[OAT-KlakSpout-013]: Made Shared Texture!");
            System.Console.WriteLine("[OAT-KlakSpout-013]: Made Shared Texture!");
        }


        public void CaptureFrame()
        {
            Update();
            if (!enabled){return;}
            if (_sourceTex != null)
            {

                // Lazy initialization for the shared texture.
                if (_sharedTexture == null)
                {
                    InitSharedTexture();
                }

                // Update the shared texture.
                if (_sharedTexture != null)
                {
                    if (_sourceTex.width != _sharedTexture.width || _sourceTex.height != _sharedTexture.height)
                    {
                        DestroyTextureAndSender();
                        return;
                    }


                    RenderTexture tmpTex = RenderTexture.GetTemporary(_sharedTexture.width, _sharedTexture.height);

                    if (_mat != null)
                    {
                        Graphics.Blit(_sourceTex, tmpTex, _mat);
                    }
                    else if (_vflip)
                    {
                        if (_blitMat == null) { _blitMat = new Material(Shader.Find("Hidden/BlitCopy")); }
                        _blitMat.mainTexture = _sourceTex;
                        var s = _blitMat.mainTextureScale;
                        s.x = 1;
                        s.y = -1;
                        _blitMat.mainTextureScale = s;

                        var o = _blitMat.mainTextureOffset;
                        o.x = 0;
                        o.y = 1;
                        _blitMat.mainTextureOffset = o;

                        Graphics.SetRenderTarget(tmpTex);
                        GL.PushMatrix();
                        if (_blitMat.SetPass(0))
                        {
                            GL.LoadOrtho();
                            GL.Clear(true,true,new Color(0,0,0,0));
                            GL.Begin(GL.QUADS);
                            GL.Vertex3(0, 0, 0);
                            GL.Vertex3(1, 0, 0);
                            GL.Vertex3(0, 1, 0);
                            GL.Vertex3(1, 1, 0);
                            GL.End();
                        }
                        GL.PopMatrix();
                    }
                    else
                    {
                        Graphics.Blit(_sourceTex, tmpTex);
                    }

                    Graphics.CopyTexture(tmpTex, _sharedTexture);

                    RenderTexture.ReleaseTemporary(tmpTex);
                }
            }
        }
    }
}
