using System;
using ColossalFramework.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    public sealed class MeshViewer : GUIWindow
    {
        private Mesh previewMesh;
        private Material previewMaterial;
        private string assetName;

        private readonly RenderTexture targetRT;

        private float m_Distance = 4f;
        private Vector2 m_PreviewDir = new Vector2(120f, -20f);
        private Bounds bounds;
        private Vector2 lastLeftMousePos = Vector2.zero;
        private Vector2 lastRightMousePos = Vector2.zero;

        private readonly Camera meshViewerCamera;

        private Material material;

        private readonly Light light;

        private bool useOriginalShader;

        private MeshViewer()
            : base("Mesh Viewer", new Rect(512, 128, 512, 512), skin)
        {
            onDraw = DrawWindow;
            onClose = () =>
            {
                SetCitizenInfoObjects(true);
                Destroy(this);
            };
            onUnityDestroy = () =>
            {
                Destroy(meshViewerCamera);
                Destroy(targetRT);
            };

            try
            {
                light = GameObject.Find("Directional Light").GetComponent<Light>();
            }
            catch (Exception)
            {
                light = null;
            }

            meshViewerCamera = gameObject.AddComponent<Camera>();
            meshViewerCamera.transform.position = new Vector3(-10000.0f, -10000.0f, -10000.0f);
            meshViewerCamera.fieldOfView = 30f;
            meshViewerCamera.backgroundColor = Color.grey;
            meshViewerCamera.nearClipPlane = 1.0f;
            meshViewerCamera.farClipPlane = 1000.0f;
            meshViewerCamera.enabled = false;
            meshViewerCamera.allowHDR = true;

            targetRT = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            meshViewerCamera.targetTexture = targetRT;
        }

        public static MeshViewer CreateMeshViewer(string assetName, Mesh mesh, Material material, bool calculateBounds = true)
        {
            var go = new GameObject("MeshViewer");
            go.transform.parent = ModTools.Instance.transform;
            var meshViewer = go.AddComponent<MeshViewer>();
            meshViewer.assetName = assetName;
            meshViewer.previewMesh = mesh;
            meshViewer.material = material;

            meshViewer.previewMaterial = new Material(Shader.Find("Diffuse"));
            if (material != null)
            {
                meshViewer.previewMaterial.mainTexture = material.mainTexture;
            }
            meshViewer.Setup(calculateBounds);
            meshViewer.visible = true;
            meshViewer.SetCitizenInfoObjects(false);
            return meshViewer;
        }

        public void Update()
        {
            if (previewMesh == null)
            {
                return;
            }

            var intensity = 0.0f;
            var color = Color.black;
            var enabled = false;

            if (light != null)
            {
                intensity = light.intensity;
                color = light.color;
                enabled = light.enabled;
                light.intensity = 2.0f;
                light.color = Color.white;
                light.enabled = true;
            }

            var magnitude = bounds.extents.magnitude;
            var num1 = magnitude + 16f;
            var num2 = magnitude * m_Distance;
            meshViewerCamera.transform.position = -Vector3.forward * num2;
            meshViewerCamera.transform.rotation = Quaternion.identity;
            meshViewerCamera.nearClipPlane = Mathf.Max(num2 - num1 * 1.5f, 0.01f);
            meshViewerCamera.farClipPlane = num2 + num1 * 1.5f;
            var q = Quaternion.Euler(m_PreviewDir.y, 0.0f, 0.0f)
                           * Quaternion.Euler(0.0f, m_PreviewDir.x, 0.0f);
            var trs = Matrix4x4.TRS(q * -bounds.center, q, Vector3.one);

            var material1 = (useOriginalShader && material != null) ? material : previewMaterial;
            Graphics.DrawMesh(previewMesh, trs, material1, 0, meshViewerCamera, 0, null, false, false);
            meshViewerCamera.RenderWithShader(material1.shader, string.Empty);

            if (light != null)
            {
                light.intensity = intensity;
                light.color = color;
                light.enabled = enabled;
            }
        }

        private void DrawWindow()
        {
            if (previewMesh != null)
            {
                title = $"Previewing \"{assetName ?? previewMesh.name}\"";

                GUILayout.BeginHorizontal();

                if (material != null)
                {
                    useOriginalShader = GUILayout.Toggle(useOriginalShader, "Original Shader");
                    if (previewMesh.isReadable)
                    {
                        if (GUILayout.Button("Dump mesh+textures", GUILayout.Width(160)))
                        {
                            DumpUtil.DumpMeshAndTextures(assetName, previewMesh, material);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Dump textures", GUILayout.Width(160)))
                        {
                            DumpUtil.DumpTextures(assetName, material);
                        }
                    }
                }
                else
                {
                    useOriginalShader = false;
                    if (previewMesh.isReadable)
                    {
                        if (GUILayout.Button("Dump mesh", GUILayout.Width(160)))
                        {
                            DumpUtil.DumpMeshAndTextures($"{previewMesh.name}", previewMesh);
                        }
                    }
                }
                if (previewMesh.isReadable)
                {
                    GUILayout.Label($"Triangles: {previewMesh.triangles.Length / 3}");
                }
                else
                {
                    var oldColor = GUI.color;
                    GUI.color = Color.yellow;
                    GUILayout.Label("Mesh isn't readable!");
                    GUI.color = oldColor;
                }
                if (material?.mainTexture != null)
                {
                    GUILayout.Label($"Texture size: {material.mainTexture.width}x{material.mainTexture.height}");
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == 0 || Event.current.button == 2)
                    {
                        lastLeftMousePos = Event.current.mousePosition;
                    }
                    else
                    {
                        lastRightMousePos = Event.current.mousePosition;
                    }
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    var pos = Event.current.mousePosition;
                    if (Event.current.button == 0 || Event.current.button == 2)
                    {
                        if (lastLeftMousePos != Vector2.zero)
                        {
                            var moveDelta = (pos - lastLeftMousePos) * 2.0f;
                            m_PreviewDir -= moveDelta / Mathf.Min(targetRT.width, targetRT.height)
                                                 * UIView.GetAView().ratio * 140f;
                            m_PreviewDir.y = Mathf.Clamp(m_PreviewDir.y, -90f, 90f);
                        }
                        lastLeftMousePos = pos;
                    }
                    else
                    {
                        if (lastRightMousePos != Vector2.zero)
                        {
                            var moveDelta1 = pos - lastRightMousePos;
                            m_Distance += (float)(moveDelta1.y / (double)targetRT.height
                                                         * UIView.GetAView().ratio * 40.0);
                            const float num1 = 6f;
                            var magnitude = bounds.extents.magnitude;
                            var num2 = magnitude + 16f;
                            m_Distance = Mathf.Min(m_Distance, 4f, num1 * (num2 / magnitude));
                        }
                        lastRightMousePos = pos;
                    }
                }

                GUI.DrawTexture(new Rect(0.0f, 64.0f, rect.width, rect.height - 64.0f), targetRT,
                    ScaleMode.StretchToFill, false);
            }
            else
            {
                title = "Mesh Viewer";
                GUILayout.Label("Use the Scene Explorer to select a Mesh for preview");
            }
        }

        public void Setup(bool calculateBounds)
        {
            if (previewMesh == null)
            {
                return;
            }

            if (calculateBounds && previewMesh.isReadable)
            {
                bounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach (var vertex in previewMesh.vertices)
                {
                    bounds.Encapsulate(vertex);
                }
            }
            else
            {
                bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(3, 3, 3));
            }
            m_Distance = 4f;
        }

        public void SetCitizenInfoObjects(bool enabled)
        {
            foreach (var i in Resources.FindObjectsOfTypeAll<CitizenInfo>())
            {
                i.gameObject.SetActive(enabled);
            }
        }
    }
}