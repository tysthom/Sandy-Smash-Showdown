using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceManager : MonoBehaviour
{
    [Header("Skin")]
    public SkinnedMeshRenderer[] athletesSkinMesh;
    public Material[] skinMaterials;

    public int athlete1SkinIndex;

    [Header("Outfits")]
    public GameObject[] maleOutfits;

    public int athlete1OutfitIndex;

    // Start is called before the first frame update
    void Start()
    {
        AssignSkin();
        AssignOutfits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void AssignSkin()
    {
        athletesSkinMesh[0].material.color = skinMaterials[athlete1SkinIndex].color;
    }

    void AssignOutfits()
    {
        SkinnedMeshRenderer[] setAthlete1Outfit = new SkinnedMeshRenderer[20];  //20 is max amount of pieces possbile on one outfit 
        SkinnedMeshRenderer[] holdAthlete1Outfit = new SkinnedMeshRenderer[maleOutfits[athlete1OutfitIndex].transform.childCount];

        for (int i = 0; i < holdAthlete1Outfit.Length; i++)
        {
            setAthlete1Outfit[i] = maleOutfits[athlete1OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            holdAthlete1Outfit[i] = Instantiate<SkinnedMeshRenderer>(setAthlete1Outfit[i]); //Assigns instaniaited versions to new array to make changes only to this version

            holdAthlete1Outfit[i].bones = athletesSkinMesh[0].bones;
            holdAthlete1Outfit[i].rootBone = athletesSkinMesh[0].rootBone;

            if (holdAthlete1Outfit[i].GetComponent<Cloth>() != null)
            {
                holdAthlete1Outfit[i].GetComponent<Cloth>().enabled = true;
            }
        }
    }
}
