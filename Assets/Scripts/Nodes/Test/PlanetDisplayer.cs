﻿using LibNoise;
using UnityEngine;
using XNode;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Debug/PlanetRender")]
    public class PlanetDisplayer : Node
    {
        [SerializeField] public Gradient grad = new Gradient();
        public PlanetProfile profile;

        public bool asChanged;
        public GameObject Water;
        public MeshRenderer WaterRend;
        Material DefaultMaterial;
        public PlanetTester Planet = null;

        [SerializeField] public int size = 512;
        public Rect TexturePosition = new Rect(14, 225, 180, 90);
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        [SerializeField] readonly float south = 90.0f;
        [SerializeField] readonly float north = -90.0f;
        [SerializeField] readonly float west = -180.0f;
        [SerializeField] readonly float east = 180.0f;

        #region UNITY

        private void Awake()
        {
            DefaultMaterial = new Material(Shader.Find("Shader Graphs/DEBUGColor"));
            InitPlanet();
            EditorSceneManager.sceneClosing += OnStartLoadingScene;
            EditorSceneManager.sceneOpened += OnLoadedScene;
        }

        private void OnDestroy()
        {
            CleanPreview();
        }

        #endregion

        public void Render()
        {
            Noise2D map = new Noise2D(
                size,
                size / 2,
                GetInputValue<SerializableModuleBase>("Input", this.Input));

                    map.GenerateSpherical(
                        south,
                        north,
                        west,
                        east);

            Planet.grad = this.grad;
            Planet.tex = new Texture2D(size, size / 2);
            Planet.tex.SetPixels(map.GetTexture(grad).GetPixels());
            Planet.tex.Apply();
            Planet.Elevate();
            SetPlanetMaterial();
            SetPlanetEffects();
            asChanged = true;
        }

        void InitPlanet()
        {
            if (Planet != null) return;

            Planet = new GameObject().AddComponent<PlanetTester>();
            Planet.gameObject.name = "Tester";
            Planet.resolution = 40;
            Planet.meanElevation = 0.15f;
            Planet.Initialize();
            Planet.transform.position = Vector3.one * 1000f;
            Planet.gameObject.hideFlags = HideFlags.HideInHierarchy;

            // +----------+ WATER

            Water = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Water.name = "water";
            WaterRend = Water.GetComponent<MeshRenderer>();

            Water.transform.SetParent(Planet.transform);
            Water.transform.localPosition = Vector3.zero;

            SetPlanetMaterial();
            SetPlanetEffects();
        }

        void OnStartLoadingScene(Scene scene, bool removingscene)
        {
            Debug.Log("Scene Name " + SceneManager.GetActiveScene().name);
            CleanPreview();
        }

        void OnLoadedScene(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            Debug.Log("Scene Name " + SceneManager.GetActiveScene().name);
            InitPlanet();
        }

        void CleanPreview()
        {
            if (Planet != null)
            {
                DestroyImmediate(Planet.gameObject);
            }
        }

        void SetPlanetMaterial()
        {
            if (DefaultMaterial != null || profile != null)
            {
                for (int i = 0; i < Planet.transform.childCount; i++)
                {
                    var child = Planet.transform.GetChild(i);

                    if (profile == null)
                    {
                        child.GetComponent<MeshRenderer>().
                            sharedMaterial =  DefaultMaterial;
                    }
                    else if (child.name != Water.name)
                    {
                        child.GetComponent<MeshRenderer>().
                            sharedMaterial = profile.material == null ? 
                                DefaultMaterial : profile.material;
                    }
                    else if (child.name == Water.name)
                    {
                        child.GetComponent<MeshRenderer>().
                            sharedMaterial = profile.WaterMaterial;
                    }
                    else if (child.name == "Aura") // TODO
                    {
                        child.GetComponent<MeshRenderer>().
                            sharedMaterial = profile.AuraMaterial;
                    }
                }
            }
        }

        void SetPlanetEffects()
        {
            if (profile == null) return;

            Water.SetActive(profile.UseWater);

            if (profile.UseWater)
            {
                Water.transform.localScale = Vector3.one * profile.SeaLevel;
                WaterRend.sharedMaterial = profile.WaterMaterial;
            }

            //Aura.SetActive(true);
            //if (profile.UseAura)
            //{
            //    Material auramat = Aura.GetComponent<MeshRenderer>().material;
            //    auramat.SetColor("Color_C8024A4F", profile.Aura);
            //    auramat.SetFloat("Vector1_733A9945", profile.AuraIntensity);
            //}
        }
    }
}