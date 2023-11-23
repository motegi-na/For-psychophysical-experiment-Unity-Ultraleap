using UnityEngine;
using Leap;
using Leap.Unity;

//  script that applies gain to hand movements from past hand data obtained with GetPastHandData

public class GainFinger : PostProcessProvider
{
    public GameObject GetPastHandData;
    GetPastHandData getPastHandData;
    [SerializeField,Range(0f, 2f)]
    private float t_Gain;
    public bool gain_Enable = false;

    Vector3[ , ] gain_pos = new Vector3[5 , 4];
    Quaternion[] past = new Quaternion[4];
    Quaternion[] current = new Quaternion[4];
    Quaternion[] inter_devide = new Quaternion[4];
    Quaternion[] difference = new Quaternion[4];
    Quaternion[ , ] gainBoneRot = new Quaternion[5 , 4];
    int flag = -1;

    public override void ProcessFrame(ref Frame inputFrame)
    {
        getPastHandData = GetPastHandData.GetComponent<GetPastHandData>();

        Hand delayRightHand = null;
        Hand delayLeftHand = null;
        foreach (var hand in inputFrame.Hands)
        {
            if (hand.IsRight)
            {
                delayRightHand = hand;
            }
            if (hand.IsLeft)
            {
                delayLeftHand = hand;
            }
        }

        if (gain_Enable == true)
        {
            flag++;
        }
        else if(gain_Enable == false)
        {
            flag = -1;
        }
        
        //right hand
        if (delayRightHand != null)
        {
            if (gain_Enable == true)
            {
                if (t_Gain <= 1)
                {
                    if (flag == 0)           //Remember information from the first frame to which gain is applied
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            for (int j = 0; j <= 3; j++)
                            {
                                gain_pos[i, j] = getPastHandData.past_Right_Finger_Bone_Center[getPastHandData.rightHand_FrameNumber][i][j];
                                gainBoneRot[i, j] = getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber][i][j];
                            }
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL).SetTransform(gain_pos[i, 0], gainBoneRot[i, 0]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL).SetTransform(gain_pos[i, 1], gainBoneRot[i, 1]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE).SetTransform(gain_pos[i, 2], gainBoneRot[i, 2]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL).SetTransform(gain_pos[i, 3], gainBoneRot[i, 3]);
                        }

                    }
                    else
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            for (int j = 0; j <= 3; j++)
                            {
                                past[j] = getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber - 1][i][j];
                                current[j] = getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber][i][j];
                                inter_devide[j] = Quaternion.Lerp(past[j], current[j], t_Gain);
                                difference[j] = inter_devide[j] * Quaternion.Inverse(past[j]);
                                gainBoneRot[i, j] = difference[j] * gainBoneRot[i, j];
                            }
                            //Overwrite using Quaternion with gain applied
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL).SetTransform(gain_pos[i, 0], gainBoneRot[i, 0]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL).SetTransform(gain_pos[i, 1], gainBoneRot[i, 1]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE).SetTransform(gain_pos[i, 2], gainBoneRot[i, 2]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL).SetTransform(gain_pos[i, 3], gainBoneRot[i, 3]);
                            //  Debug.Log("gaingain");
                        }
                    }
                }

                if (t_Gain > 1)
                {
                    if (flag == 0)
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            for (int j = 0; j <= 3; j++)
                            {
                                gain_pos[i, j] = getPastHandData.past_Right_Finger_Bone_Center[getPastHandData.rightHand_FrameNumber][i][j];
                                gainBoneRot[i, j] = getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber][i][j];
                            }
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL).SetTransform(gain_pos[i, 0], gainBoneRot[i, 0]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL).SetTransform(gain_pos[i, 1], gainBoneRot[i, 1]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE).SetTransform(gain_pos[i, 2], gainBoneRot[i, 2]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL).SetTransform(gain_pos[i, 3], gainBoneRot[i, 3]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            for (int j = 0; j <= 3; j++)
                            {
                                past[j] = getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber - 1][i][j];
                                current[j] = getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber][i][j];
                                inter_devide[j] = Quaternion.Lerp(past[j], current[j], t_Gain / 2);
                                difference[j] = inter_devide[j] * Quaternion.Inverse(past[j]);
                                gainBoneRot[i, j] = difference[j] * difference[j] * gainBoneRot[i, j];
                            }
                            //Overwrite using Quaternion with gain applied
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL).SetTransform(gain_pos[i, 0], gainBoneRot[i, 0]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL).SetTransform(gain_pos[i, 1], gainBoneRot[i, 1]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE).SetTransform(gain_pos[i, 2], gainBoneRot[i, 2]);
                            delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL).SetTransform(gain_pos[i, 3], gainBoneRot[i, 3]);
                        }
                    }
                }
            }
        }
    }
}
