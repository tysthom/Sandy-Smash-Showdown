using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceManager : MonoBehaviour
{
    [Header("Skin")]
    public SkinnedMeshRenderer athlete1AISkin;
    public SkinnedMeshRenderer[] athletesSkinMesh;
    public Material[] skinMaterials;

    public int athlete1SkinIndex;
    public int athlete2SkinIndex;
    public int athlete3SkinIndex;
    public int athlete4SkinIndex;

    [Header("Outfits")]
    public GameObject[] maleTeam1Outfits;
    public GameObject[] maleTeam2Outfits;

    public int athlete1OutfitIndex;
    public int athlete2OutfitIndex;
    public int athlete3OutfitIndex;
    public int athlete4OutfitIndex;

    public void AssignSkin()
    {
        athletesSkinMesh[0].material.color = skinMaterials[athlete1SkinIndex].color;
        athletesSkinMesh[1].material.color = skinMaterials[athlete2SkinIndex].color;
        athletesSkinMesh[2].material.color = skinMaterials[athlete3SkinIndex].color;
        athletesSkinMesh[3].material.color = skinMaterials[athlete4SkinIndex].color;
    }

    public void AssignOutfits()
    {
        //Athlete 1
        SkinnedMeshRenderer[] setAthleteOutfit = new SkinnedMeshRenderer[20];  //20 is max amount of pieces possbile on one outfit 
        SkinnedMeshRenderer[] holdAthleteOutfit = new SkinnedMeshRenderer[maleTeam1Outfits[athlete1OutfitIndex].transform.childCount];

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = maleTeam1Outfits[athlete1OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            holdAthleteOutfit[i] = Instantiate<SkinnedMeshRenderer>(setAthleteOutfit[i]); //Assigns instaniaited versions to new array to make changes only to this version

            holdAthleteOutfit[i].bones = athletesSkinMesh[0].bones;
            holdAthleteOutfit[i].rootBone = athletesSkinMesh[0].rootBone;

            if (holdAthleteOutfit[i].GetComponent<Cloth>() != null)
            {
                holdAthleteOutfit[i].GetComponent<Cloth>().enabled = true;
            }
        }

        //Athlete 2
        setAthleteOutfit = new SkinnedMeshRenderer[20];  //20 is max amount of pieces possbile on one outfit 
        holdAthleteOutfit = new SkinnedMeshRenderer[maleTeam1Outfits[athlete2OutfitIndex].transform.childCount];

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = maleTeam1Outfits[athlete2OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            holdAthleteOutfit[i] = Instantiate<SkinnedMeshRenderer>(setAthleteOutfit[i]); //Assigns instaniaited versions to new array to make changes only to this version

            holdAthleteOutfit[i].bones = athletesSkinMesh[1].bones;
            holdAthleteOutfit[i].rootBone = athletesSkinMesh[1].rootBone;

            if (holdAthleteOutfit[i].GetComponent<Cloth>() != null)
            {
                holdAthleteOutfit[i].GetComponent<Cloth>().enabled = true;
            }
        }


        //Athlete 3
        setAthleteOutfit = new SkinnedMeshRenderer[20];  //20 is max amount of pieces possbile on one outfit 
        holdAthleteOutfit = new SkinnedMeshRenderer[maleTeam2Outfits[athlete3OutfitIndex].transform.childCount];

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = maleTeam2Outfits[athlete3OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            holdAthleteOutfit[i] = Instantiate<SkinnedMeshRenderer>(setAthleteOutfit[i]); //Assigns instaniaited versions to new array to make changes only to this version

            holdAthleteOutfit[i].bones = athletesSkinMesh[2].bones;
            holdAthleteOutfit[i].rootBone = athletesSkinMesh[2].rootBone;

            if (holdAthleteOutfit[i].GetComponent<Cloth>() != null)
            {
                holdAthleteOutfit[i].GetComponent<Cloth>().enabled = true;
            }
        }

        //Athlete 4
        setAthleteOutfit = new SkinnedMeshRenderer[20];  //20 is max amount of pieces possbile on one outfit 
        holdAthleteOutfit = new SkinnedMeshRenderer[maleTeam2Outfits[athlete4OutfitIndex].transform.childCount];

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = maleTeam2Outfits[athlete4OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            holdAthleteOutfit[i] = Instantiate<SkinnedMeshRenderer>(setAthleteOutfit[i]); //Assigns instaniaited versions to new array to make changes only to this version

            holdAthleteOutfit[i].bones = athletesSkinMesh[3].bones;
            holdAthleteOutfit[i].rootBone = athletesSkinMesh[3].rootBone;

            if (holdAthleteOutfit[i].GetComponent<Cloth>() != null)
            {
                holdAthleteOutfit[i].GetComponent<Cloth>().enabled = true;
            }
        }
    }
}
