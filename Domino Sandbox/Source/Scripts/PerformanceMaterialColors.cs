using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PerformanceMaterialColors
{ 
    private class MaterialColor
    {
        private Material mat;
        private Color col;

        public Material material
        {
            get
            {
                return mat;
            }
        }
        public Color color
        {
            get
            {
                return col;
            }
        }

        public MaterialColor(Material material, Color color)
        {
            mat = material;
            mat.color = color;
            col = color;
        }
    }

    // Two dimensional generic lists are possible (for supporting multiple materials for this batching optimizer)
    // private static List<List<MaterialColor>> intantiatedMaterials;
    private static List<MaterialColor> instantiatedMaterials = new List<MaterialColor>();

    public static Material GetMaterial(Renderer rend, Color col, int whichMat)
    {
        foreach(MaterialColor matcol in instantiatedMaterials)
        {
            if(matcol.color == col)
            {
                return matcol.material;
            }
        }
        Material refMat = new Material(rend.materials[whichMat]);
        refMat.color = col;
        instantiatedMaterials.Add(new MaterialColor(refMat, col));

        return instantiatedMaterials[instantiatedMaterials.Count - 1].material;
    }
}
