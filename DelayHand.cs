using UnityEngine;
using Leap;
using Leap.Unity;

// A script that substitutes past hand data obtained with GetPastHandData and delays fingers

public class DelayHand : PostProcessProvider
{
    public int right_DelayFrame = 10;
    public int left_DelayFrame = 10;

    public override void ProcessFrame(ref Frame inputFrame)
    {
        //Load hand data from script GetPastHandData
        GetPastHandData getPastHandData;
        GameObject gameObject = GameObject.Find("GetPastHandData");
        getPastHandData = gameObject.GetComponent<GetPastHandData>();

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

        //right hand
        if (delayRightHand != null)
        {
            if (getPastHandData.rightHand_FrameNumber >= right_DelayFrame) //When past data is accumulated
            {
                //Overwrite DelayFrame with the data of the previous frame and delay it
                for (int i = 0; i <= 4; i++)
                    {
                        delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL).SetTransform(getPastHandData.past_Right_Finger_Bone_Center[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][0],
                        getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][0]);

                        delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL).SetTransform(getPastHandData.past_Right_Finger_Bone_Center[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][1],
                        getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][1]);

                        delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE).SetTransform(getPastHandData.past_Right_Finger_Bone_Center[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][2],
                        getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][2]);

                        delayRightHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL).SetTransform(getPastHandData.past_Right_Finger_Bone_Center[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][3],
                        getPastHandData.past_Right_Finger_Bone_Rotation[getPastHandData.rightHand_FrameNumber - right_DelayFrame][i][3]);
                    }
                delayRightHand.SetTransform(getPastHandData.past_Right_PalmPosition[getPastHandData.rightHand_FrameNumber - right_DelayFrame], getPastHandData.past_Right_Rotation[getPastHandData.rightHand_FrameNumber - right_DelayFrame]);
            }
        }

        //left hand
        if (delayLeftHand != null)
        {
            if (getPastHandData.leftHand_FrameNumber >= left_DelayFrame)
            {
                for (int i = 0; i <= 4; i++)
                {
                    delayLeftHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL).SetTransform(getPastHandData.past_Left_Finger_Bone_Center[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][0],
                    getPastHandData.past_Left_Finger_Bone_Rotation[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][0]);

                    delayLeftHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL).SetTransform(getPastHandData.past_Left_Finger_Bone_Center[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][1],
                    getPastHandData.past_Left_Finger_Bone_Rotation[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][1]);

                    delayLeftHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE).SetTransform(getPastHandData.past_Left_Finger_Bone_Center[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][2],
                    getPastHandData.past_Left_Finger_Bone_Rotation[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][2]);

                    delayLeftHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL).SetTransform(getPastHandData.past_Left_Finger_Bone_Center[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][3],
                    getPastHandData.past_Left_Finger_Bone_Rotation[getPastHandData.leftHand_FrameNumber - left_DelayFrame][i][3]);

                }
                delayLeftHand.SetTransform(getPastHandData.past_Left_PalmPosition[getPastHandData.leftHand_FrameNumber - left_DelayFrame], getPastHandData.past_Left_Rotation[getPastHandData.leftHand_FrameNumber - left_DelayFrame]);
            }
        }
    }
}
