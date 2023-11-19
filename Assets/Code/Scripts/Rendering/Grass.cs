using System;
using UnityEngine;

namespace Airhead.Runtime.Rendering
{
    [ExecuteAlways]
    [SelectionBase, DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    public sealed class Grass : MonoBehaviour
    {
        public Material material;
        public int submesh = 0;
        public int shells = 16;

        private MeshFilter filter;
        private ComputeBuffer buffer;

        private void Update()
        {
            if (!filter) filter = GetComponent<MeshFilter>();

            if (!filter) return;
            if (!material) return;
            
            var data = new Matrix4x4[shells];
            for (var i = 0; i < shells; i++)
            {
                var p = (i + 1.0f) / shells;
                data[i] = transform.localToWorldMatrix;
            }

            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetInt("_ShellCount", shells);
            
            Graphics.DrawMeshInstanced(filter.mesh, submesh, material, data, shells, propertyBlock);
        }
    }
}