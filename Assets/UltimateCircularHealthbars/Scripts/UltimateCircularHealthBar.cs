using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace RengeGames.HealthBars {

    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("Health Bars/Circular Segmented Health Bar")]
    public class UltimateCircularHealthBar : MonoBehaviour, ISegmentedHealthBar {
        [Header("Data")] public float SegmentCount = 5f;
        public float RemovedSegments = 0f;

        [Header("Appearance")] [Range(0, 1)] public float Spacing = 0.04f;
        public Color Color;
        [Range(0, 1)] public float Radius = 0.4f;
        [Range(0, 1)] public float LineWidth = 0.04f;
        [Range(0, 360)] public float Rotation = 0;

        private Color oldColor;
        private float oldSegmentCount;
        private float oldRemovedSegments;
        private float oldSpacing;
        private float oldRadius;
        private float oldLineWidth;
        private float oldRotation;

        private int colorID;
        private int segmentCountID;
        private int removedSegmentsID;
        private int spacingID;
        private int radiusID;
        private int linewidthID;
        private int rotationID;

        private Material currentMaterial;

        private bool materialAssigned = false;
        private const string MATERIAL_NAME = "radialSegmentedHealthBarInstance";

        private static string BaseMaterialName {
            get {
                if (GraphicsSettings.renderPipelineAsset && Int32.Parse(Application.unityVersion.Split('.')[0]) > 2018)
                    return "RadialSegmentedHealthBarMaterial";

                return "RadialSegmentedHealthBarBuiltInMaterial";
            }
        }

        private void Awake() {

            colorID = Shader.PropertyToID("_Color");
            segmentCountID = Shader.PropertyToID("_SegmentCount");
            removedSegmentsID = Shader.PropertyToID("_RemoveSegments");
            spacingID = Shader.PropertyToID("_SegmentSpacing");
            radiusID = Shader.PropertyToID("_Radius");
            linewidthID = Shader.PropertyToID("_LineWidth");
            rotationID = Shader.PropertyToID("_Rotation");

#if UNITY_EDITOR
            SpriteRenderer r = GetComponent<SpriteRenderer>();
            if (r != null) {
                AssignMaterial(r);
            } else {
                Image img = GetComponent<Image>();
                if (img != null) {
                    AssignMaterial(img);
                }
            }

            SetMaterialPropertyIfChanged(ref oldColor, Color, colorID);
            SetMaterialPropertyIfChanged(ref oldSegmentCount, SegmentCount, segmentCountID);
            SetMaterialPropertyIfChanged(ref oldRemovedSegments, RemovedSegments, removedSegmentsID);
            SetMaterialPropertyIfChanged(ref oldSpacing, Spacing, spacingID);
            SetMaterialPropertyIfChanged(ref oldRadius, Radius, radiusID);
            SetMaterialPropertyIfChanged(ref oldLineWidth, LineWidth, linewidthID);
            SetMaterialPropertyIfChanged(ref oldRotation, Rotation, rotationID);
#endif
        }

        private void Start() {

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Image img = GetComponent<Image>();
            if (sr != null) {
                currentMaterial = sr.sharedMaterial;
            } else if (img != null) {
                currentMaterial = img.material;
            }

            if (currentMaterial != null && currentMaterial.name == MATERIAL_NAME) {
                materialAssigned = true;
            }
        }

        private void Update() {
#if UNITY_EDITOR
            if (!materialAssigned && !Application.isPlaying) {
                SpriteRenderer r = GetComponent<SpriteRenderer>();
                if (r != null) {
                    if (r.sharedMaterial == null || r.sharedMaterial.name != MATERIAL_NAME) {
                        AssignMaterial(r);

                    } else {
                        materialAssigned = true;
                        currentMaterial = r.material;
                    }
                    ResetPublicFields();
                } else {
                    Image img = GetComponent<Image>();
                    if (img != null) {
                        if (img.material.name != MATERIAL_NAME) AssignMaterial(img);
                        else {
                            materialAssigned = true;
                            currentMaterial = img.material;
                        }
                        ResetPublicFields();
                    }
                }
            }
#endif
            if (materialAssigned) {
                SetMaterialPropertyIfChanged(ref oldColor, Color, colorID);
                SetMaterialPropertyIfChanged(ref oldSegmentCount, SegmentCount, segmentCountID);
                SetMaterialPropertyIfChanged(ref oldRemovedSegments, RemovedSegments, removedSegmentsID);
                SetMaterialPropertyIfChanged(ref oldSpacing, Spacing, spacingID);
                SetMaterialPropertyIfChanged(ref oldRadius, Radius, radiusID);
                SetMaterialPropertyIfChanged(ref oldLineWidth, LineWidth, linewidthID);
                SetMaterialPropertyIfChanged(ref oldRotation, Rotation, rotationID);
            }

        }

        void SetMaterialPropertyIfChanged(ref float old, float val, int propertyID) {
            if (old != val) {
                old = val;
                currentMaterial.SetFloat(propertyID, val);
            }
        }

        void SetMaterialPropertyIfChanged(ref Color old, Color val, int propertyID) {
            if (old != val) {
                old = val;
                currentMaterial.SetColor(propertyID, val);
            }
        }


        public void AssignMaterial(Image r) {
            //get material
            Material mat = Resources.Load<Material>(BaseMaterialName);

            if (Application.isEditor && mat != null && r != null) {
                //generate and apply the material
                currentMaterial = new Material(mat);
                currentMaterial.name = MATERIAL_NAME;
                r.material = currentMaterial;
                materialAssigned = true;
#if UNITY_EDITOR
                //the scene needs to be saved
                if (!Application.isPlaying)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
            } else {
                //something went wrong, remove the component
                Debug.LogError("RadialSegmentedHealthBar: Something went wrong.");
                DestroyImmediate(this);
            }
        }

        public void AssignMaterial(SpriteRenderer r) {
            //get resources
            Material mat = Resources.Load<Material>(BaseMaterialName);
            Sprite sprite = Resources.Load<Sprite>("placeholderSprite");

            if (Application.isEditor && mat != null && r != null) {
                //make sure the sprite will render the shader correctly
                if (r.sprite == null && sprite != null) {
                    r.sprite = sprite;
                }

                r.drawMode = SpriteDrawMode.Simple;

                //generate and apply the material
                currentMaterial = new Material(mat);
                currentMaterial.name = MATERIAL_NAME;
                r.sharedMaterial = currentMaterial;
                materialAssigned = true;
#if UNITY_EDITOR
                //the scene needs to be saved
                if (!Application.isPlaying)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
            } else {
                //something went wrong, remove the component
                Debug.LogError("RadialSegmentedHealthBar: Something went wrong.");
                DestroyImmediate(this);
            }
        }

        public void ResetPublicFields() {
            oldColor = Color = currentMaterial.GetColor(colorID);
            oldSegmentCount = SegmentCount = currentMaterial.GetFloat(segmentCountID);
            oldRemovedSegments = RemovedSegments = currentMaterial.GetFloat(removedSegmentsID);
            oldSpacing = Spacing = currentMaterial.GetFloat(spacingID);
            oldRadius = Radius = currentMaterial.GetFloat(radiusID);
            oldLineWidth = LineWidth = currentMaterial.GetFloat(linewidthID);
            oldRotation = Rotation = currentMaterial.GetFloat(rotationID);
        }

        public void SetSegmentCount(float value) {
            SegmentCount = Mathf.Max(0, value);
        }

        public void SetRemovedSegments(float value) {
            RemovedSegments = Mathf.Clamp(value, 0, SegmentCount);
        }

        public void SetPercent(float value) {
            float cVal = Mathf.Clamp(value, 0, 1);
            RemovedSegments = (1 - cVal) * SegmentCount;
        }

        public void AddRemoveSegments(float value) {
            RemovedSegments += value;
            RemovedSegments = Mathf.Clamp(RemovedSegments, 0, SegmentCount);
        }

        public void AddRemovePercent(float value) {
            RemovedSegments += value * SegmentCount;
            RemovedSegments = Mathf.Clamp(RemovedSegments, 0, SegmentCount);
        }
    }
}