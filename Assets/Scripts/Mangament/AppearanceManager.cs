using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceManager : MonoBehaviour
{
    [Header("Body")]
    public int[] athletesBodyType;
    //0 - Male, 1 - Female

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
    public GameObject[] femaleTeam1Outfits;
    public GameObject[] maleTeam2Outfits;
    public GameObject[] femaleTeam2Outfits;

    public int athlete1OutfitIndex;
    public int athlete2OutfitIndex;
    public int athlete3OutfitIndex;
    public int athlete4OutfitIndex;

    [Header("Hair")]
    public GameObject[] maleHair;
    public GameObject[] femaleHair;

    public int[] athleteHairIndices;

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
        SkinnedMeshRenderer[] holdAthleteOutfit = (athletesBodyType[0] == 0 ?
            new SkinnedMeshRenderer[maleTeam1Outfits[athlete1OutfitIndex].transform.childCount] 
            : new SkinnedMeshRenderer[femaleTeam1Outfits[athlete1OutfitIndex].transform.childCount]);

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = (athletesBodyType[0] == 0 ? 
                maleTeam1Outfits[athlete1OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() 
                : femaleTeam1Outfits[athlete1OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>());

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
        holdAthleteOutfit = (athletesBodyType[1] == 0 ?
            new SkinnedMeshRenderer[maleTeam1Outfits[athlete2OutfitIndex].transform.childCount]
            : new SkinnedMeshRenderer[femaleTeam1Outfits[athlete2OutfitIndex].transform.childCount]);

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = (athletesBodyType[1] == 0 ?
                maleTeam1Outfits[athlete2OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>()
                : femaleTeam1Outfits[athlete2OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>());

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
        holdAthleteOutfit = (athletesBodyType[2] == 0 ?
            new SkinnedMeshRenderer[maleTeam2Outfits[athlete3OutfitIndex].transform.childCount]
            : new SkinnedMeshRenderer[femaleTeam2Outfits[athlete3OutfitIndex].transform.childCount]);

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = (athletesBodyType[2] == 0 ?
                maleTeam2Outfits[athlete3OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>()
                : femaleTeam2Outfits[athlete3OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>());

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
        holdAthleteOutfit = (athletesBodyType[3] == 0 ?
            new SkinnedMeshRenderer[maleTeam2Outfits[athlete4OutfitIndex].transform.childCount]
            : new SkinnedMeshRenderer[femaleTeam2Outfits[athlete4OutfitIndex].transform.childCount]);

        for (int i = 0; i < holdAthleteOutfit.Length; i++)
        {
            setAthleteOutfit[i] = (athletesBodyType[3] == 0 ?
                maleTeam2Outfits[athlete4OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>()
                : femaleTeam2Outfits[athlete4OutfitIndex].transform.GetChild(i).GetComponent<SkinnedMeshRenderer>());

            holdAthleteOutfit[i] = Instantiate<SkinnedMeshRenderer>(setAthleteOutfit[i]); //Assigns instaniaited versions to new array to make changes only to this version

            holdAthleteOutfit[i].bones = athletesSkinMesh[3].bones;
            holdAthleteOutfit[i].rootBone = athletesSkinMesh[3].rootBone;

            if (holdAthleteOutfit[i].GetComponent<Cloth>() != null)
            {
                holdAthleteOutfit[i].GetComponent<Cloth>().enabled = true;
            }
        }
    }

    public void AssignHair()
    {
        for(int i = 0; i < GetComponent<GameManager>().athletes.Length; i += 1)
        {
            int hairIndex = athleteHairIndices[i];
            if (athleteHairIndices[i] == 0)
            {
                //Bald
                
            }
            else
            {
                GameObject hairHolder;

                hairHolder = GetComponent<GameManager>().athletes[i].GetComponent<AthleteStatus>().hairHolder;  //Assign hairholder variable to gameObject    
                if (athletesBodyType[i] == 0)    //Assigns male hair type 
                {
                    GameObject designatedHair = Instantiate(maleHair[hairIndex], hairHolder.transform); //Instantiates selected hair
                    designatedHair.transform.localPosition = Vector3.zero;
                    designatedHair.transform.localRotation = Quaternion.identity;
                }
                else
                {

                    GameObject designatedHair = Instantiate(femaleHair[hairIndex], hairHolder.transform); //Instantiates selected hair
                    designatedHair.transform.localPosition = Vector3.zero;
                    designatedHair.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
}
