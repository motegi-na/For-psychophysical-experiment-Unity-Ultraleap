using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

 //Script that stores acquired hand data in a List for each frame
 
public class GetPastHandData : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider;
    public int rightHand_FrameNumber = -1;               //Number of frames since data acquisition started
    public int leftHand_FrameNumber = -1;

    //↓↓↓Hand List      Lists that stores hand data for each frame that can be obtained using Leapmotion's functions.
    public int right_ID;                   
    public int left_ID;
    public List<Vector3> past_Right_PalmPosition = new List<Vector3>();       //center position of palm
    public List<Vector3> past_Left_PalmPosition = new List<Vector3>();     
    public List<Vector3> past_Right_PalmVelocity = new List<Vector3>();       //palm speed
    public List<Vector3> past_Left_PalmVelocity = new List<Vector3>();      
    public List<Vector3> past_Right_PalmNormal = new List<Vector3>();         //Palm normal vector
    public List<Vector3> past_Left_PalmNormal = new List<Vector3>();         
    public List<Vector3> past_Right_PalmDirection = new List<Vector3>();       //Direction from palm to fingers
    public List<Vector3> past_Left_PalmDirection = new List<Vector3>();      
    public List<Quaternion> past_Right_Rotation = new List<Quaternion>();         //hand rotation
    public List<Quaternion> past_Left_Rotation = new List<Quaternion>();    
    public List<float> past_Right_GrabStrength = new List<float>();          //Grip strength 0-1
    public List<float> past_Left_GrabStrength = new List<float>();         
    public List<float> past_Right_PinchStrength = new List<float>();          //Pinch strength 0-1 Thumb and other fingers
    public List<float> past_Left_PinchStrength = new List<float>();          
              
    public List<float> past_Right_PalmWidth = new List<float>();              //width of palm
    public List<float> past_Left_PalmWidth = new List<float>();            
    public List<Vector3> past_Right_WristPosition = new List<Vector3>();       //wrist position
    public List<Vector3> past_Left_WristPosition = new List<Vector3>();      
    public float right_TimeVisible;           
    public float left_TimeVisible;
    public List<float> past_Right_Confidence = new List<float>();            //Reliability of hand pose: 0-1
    public List<float> past_Left_Confidence = new List<float>();

    //Pinch data
    public List<float> past_Right_PinchDistance = new List<float>();          //distance between thumb and index finger
    public List<float> past_Left_PinchDistance = new List<float>();

    //↓↓↓Finger List
    //right finger
    public List<Finger> right_Finger;
    public Finger.FingerType[] right_FingerType = new Finger.FingerType[5];
    public int[] right_Finger_ID = new int[5];
    public int[] right_Finger_HandID = new int[5];
    public List<List<Vector3>> past_Right_Finger_TipPosition = new List<List<Vector3>>();
    public List<List<Vector3>> past_Right_Finger_Direction = new List<List<Vector3>>();
    public List<List<float>> past_Right_Finger_Width = new List<List<float>>();
    public List<List<float>> past_Right_Finger_Length = new List<List<float>>();
    public bool[] right_ISExtended = new bool[5];

    //left finger
    public List<Finger> left_Finger;
    public Finger.FingerType[] left_FingerType = new Finger.FingerType[5];                 //Finger type
    public int[] left_Finger_ID = new int[5];                                             //Finger ID
    public int[] left_Finger_HandID = new int[5];                                         //ID of the hand that the finger is attached to
    public List<List<Vector3>> past_Left_Finger_TipPosition = new List<List<Vector3>>();    //fingertip coordinates
    public List<List<Vector3>> past_Left_Finger_Direction = new List<List<Vector3>>();      //Fingertip direction
    public List<List<float>> past_Left_Finger_Width = new List<List<float>>();             //finger width
    public List<List<float>> past_Left_Finger_Length = new List<List<float>>();            //finger length
    public bool[] left_ISExtended = new bool[5];                                          //Is the finger extended?

    //↓↓↓Bone List
    //right finger bone
    public Bone[,] right_Finger_Bone = new Bone[5, 4];                         //5 fingers x 4 bones
    public Bone.BoneType[,] right_Finger_Bone_Type = new Bone.BoneType[5, 4];  //bone type
    //　frame×finger×bone
    public List<List<List<Vector3>>> past_Right_Finger_Bone_NextJoint = new List<List<List<Vector3>>>();        //Wrist side of bone
    public List<List<List<Vector3>>> past_Right_Finger_Bone_Center = new List<List<List<Vector3>>>();           //middle of bone
    public List<List<List<Vector3>>> past_Right_Finger_Bone_PrevJoint = new List<List<List<Vector3>>>();        //fingertip side of bone
    public List<List<List<Vector3>>> past_Right_Finger_Bone_Direction = new List<List<List<Vector3>>>();        //bone direction
    public List<List<List<float>>> past_Right_Finger_Bone_Length = new List<List<List<float>>>();               //bone length
    public List<List<List<float>>> past_Right_Finger_Bone_Width = new List<List<List<float>>>();                //Average width of meat around the bone?
    public List<List<List<Quaternion>>> past_Right_Finger_Bone_Rotation = new List<List<List<Quaternion>>>();   //bone rotation
    //left finger bone　　　　　　　　　　　　　　　　　　　　 
    public Bone[,] left_Finger_Bone = new Bone[5, 4];        
    public Bone.BoneType[,] left_Finger_Bone_Type = new Bone.BoneType[5, 4];    
    public List<List<List<Vector3>>> past_Left_Finger_Bone_NextJoint = new List<List<List<Vector3>>>();        
    public List<List<List<Vector3>>> past_Left_Finger_Bone_Center = new List<List<List<Vector3>>>();       
    public List<List<List<Vector3>>> past_Left_Finger_Bone_PrevJoint = new List<List<List<Vector3>>>();        
    public List<List<List<Vector3>>> past_Left_Finger_Bone_Direction = new List<List<List<Vector3>>>();       
    public List<List<List<float>>> past_Left_Finger_Bone_Length = new List<List<List<float>>>();        
    public List<List<List<float>>> past_Left_Finger_Bone_Width = new List<List<List<float>>>();          
    public List<List<List<Quaternion>>> past_Left_Finger_Bone_Rotation = new List<List<List<Quaternion>>>();    


    // Update is called once per frame
    void Update()
    {
        Frame frame = LeapServiceProvider.CurrentFrame;    //get frame
        Hand rightHand = null;
        Hand leftHand = null;
        foreach (Hand hand in frame.Hands)　　　　　　　　
        {
            if (hand.IsRight)
            {
                rightHand = hand;
            }
            if (hand.IsLeft)
            {
                leftHand = hand;
            }
        }

        if (rightHand == null)     //Initialize past data when hands are no longer visible
        {  
            rightHand_FrameNumber = -1;
            //Hand
            past_Right_PalmPosition = new List<Vector3>();
            past_Right_PalmVelocity = new List<Vector3>();   
            past_Right_PalmNormal = new List<Vector3>();         
            past_Right_PalmDirection = new List<Vector3>();      
            past_Right_Rotation = new List<Quaternion>();        
            past_Right_GrabStrength = new List<float>();         
            past_Right_PinchStrength = new List<float>();       
            past_Right_PinchDistance = new List<float>();        
            past_Right_PalmWidth = new List<float>();            
            past_Right_WristPosition = new List<Vector3>();      
            past_Right_Confidence = new List<float>();
            //finger
            past_Right_Finger_TipPosition = new List<List<Vector3>>();
            past_Right_Finger_Direction = new List<List<Vector3>>();
            past_Right_Finger_Width = new List<List<float>>();
            past_Right_Finger_Length = new List<List<float>>();
            //Bone
            past_Right_Finger_Bone_NextJoint = new List<List<List<Vector3>>>();
            past_Right_Finger_Bone_Center = new List<List<List<Vector3>>>();
            past_Right_Finger_Bone_PrevJoint = new List<List<List<Vector3>>>();
            past_Right_Finger_Bone_Direction = new List<List<List<Vector3>>>();
            past_Right_Finger_Bone_Length = new List<List<List<float>>>();
            past_Right_Finger_Bone_Width = new List<List<List<float>>>();       
            past_Right_Finger_Bone_Rotation = new List<List<List<Quaternion>>>();
         }

        if (rightHand != null)
        {
            //When hands are displayed, data is stored in a list for each frame
            //↓↓↓Hand data
            rightHand_FrameNumber++;
          //  Debug.Log(rightHand_FrameNumber);
            right_ID = rightHand.Id;
            past_Right_PalmPosition.Add(rightHand.PalmPosition.ToVector3());
            past_Right_PalmVelocity.Add(rightHand.PalmVelocity.ToVector3());
            past_Right_PalmNormal.Add(rightHand.PalmNormal.ToVector3());
            past_Right_PalmDirection.Add(rightHand.Direction.ToVector3());
            past_Right_Rotation.Add(rightHand.Rotation.ToQuaternion());
            past_Right_GrabStrength.Add(rightHand.GrabStrength);
            past_Right_PinchStrength.Add(rightHand.PinchStrength);
            past_Right_PinchDistance.Add(rightHand.PinchDistance);
            past_Right_PalmWidth.Add(rightHand.PalmWidth);
            past_Right_WristPosition.Add(rightHand.WristPosition.ToVector3());
            right_TimeVisible = rightHand.TimeVisible;
            past_Right_Confidence.Add(rightHand.Confidence);

            //Initialize the second dimension of the finger list
            past_Right_Finger_TipPosition.Add(new List<Vector3>());
            past_Right_Finger_TipPosition.Add(new List<Vector3>());
            past_Right_Finger_Direction.Add(new List<Vector3>());
            past_Right_Finger_Width.Add(new List<float>());
            past_Right_Finger_Length.Add(new List<float>());

            //↓↓↓finger data
            right_Finger = rightHand.Fingers;
            for (int i = 0; i <= 4; i++)
            {
                right_FingerType[i] = rightHand.Fingers[i].Type;
                right_Finger_ID[i] = rightHand.Fingers[i].Id;
                right_Finger_HandID[i] = rightHand.Fingers[i].HandId;
                past_Right_Finger_TipPosition[rightHand_FrameNumber].Add(rightHand.Fingers[i].TipPosition.ToVector3());
                past_Right_Finger_Direction[rightHand_FrameNumber].Add(rightHand.Fingers[i].Direction.ToVector3());
                past_Right_Finger_Width[rightHand_FrameNumber].Add(rightHand.Fingers[i].Width);
                past_Right_Finger_Length[rightHand_FrameNumber].Add(rightHand.Fingers[i].Length);
                right_ISExtended[i] = rightHand.Fingers[i].IsExtended;
            }
            //↓↓↓Bone data
            //Initialize the second dimension of the bone list
            past_Right_Finger_Bone_NextJoint.Add(new List<List<Vector3>>());
            past_Right_Finger_Bone_Center.Add(new List<List<Vector3>>());
            past_Right_Finger_Bone_PrevJoint.Add(new List<List<Vector3>>());
            past_Right_Finger_Bone_Direction.Add(new List<List<Vector3>>());
            past_Right_Finger_Bone_Length.Add(new List<List<float>>());
            past_Right_Finger_Bone_Width.Add(new List<List<float>>());
            past_Right_Finger_Bone_Rotation.Add(new List<List<Quaternion>>());

            for (int i = 0; i <= 4; i++)      //finger×5
            {
                //Initialize the third dimension of the bone list
                past_Right_Finger_Bone_NextJoint[rightHand_FrameNumber].Add(new List<Vector3>());
                past_Right_Finger_Bone_Center[rightHand_FrameNumber].Add(new List<Vector3>());
                past_Right_Finger_Bone_PrevJoint[rightHand_FrameNumber].Add(new List<Vector3>());
                past_Right_Finger_Bone_Direction[rightHand_FrameNumber].Add(new List<Vector3>());
                past_Right_Finger_Bone_Length[rightHand_FrameNumber].Add(new List<float>());
                past_Right_Finger_Bone_Width[rightHand_FrameNumber].Add(new List<float>());
                past_Right_Finger_Bone_Rotation[rightHand_FrameNumber].Add(new List<Quaternion>());

                for (int j = 0; j <= 3; j++)  //bone×4
                {
                    //get bone
                    if (j == 0)
                    {
                        right_Finger_Bone[i, j] = rightHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL);
                    }
                    else if (j == 1)
                    {
                        right_Finger_Bone[i, j] = rightHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL);
                    }
                    else if (j == 2)
                    {
                        right_Finger_Bone[i, j] = rightHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE);
                    }
                    else if (j == 3)
                    {
                        right_Finger_Bone[i, j] = rightHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL);
                    }
                    //Substitute parameters for each bone from the obtained bone
                    right_Finger_Bone_Type[i, j] = right_Finger_Bone[i, j].Type;
                    past_Right_Finger_Bone_NextJoint[rightHand_FrameNumber][i].Add(right_Finger_Bone[i,j].NextJoint.ToVector3());
                    past_Right_Finger_Bone_Center[rightHand_FrameNumber][i].Add(right_Finger_Bone[i, j].Center.ToVector3());
                    past_Right_Finger_Bone_PrevJoint[rightHand_FrameNumber][i].Add(right_Finger_Bone[i, j].PrevJoint.ToVector3());
                    past_Right_Finger_Bone_Direction[rightHand_FrameNumber][i].Add(right_Finger_Bone[i, j].Direction.ToVector3());
                    past_Right_Finger_Bone_Length[rightHand_FrameNumber][i].Add(right_Finger_Bone[i, j].Length);
                    past_Right_Finger_Bone_Width[rightHand_FrameNumber][i].Add(right_Finger_Bone[i, j].Width);
                    past_Right_Finger_Bone_Rotation[rightHand_FrameNumber][i].Add(right_Finger_Bone[i, j].Rotation.ToQuaternion());
                }
            }
        }

        if (leftHand == null)       //Initialize past data when hands are no longer visible
        {　 
            leftHand_FrameNumber = -1;
            //hand
            past_Left_PalmPosition = new List<Vector3>();
            past_Left_PalmVelocity = new List<Vector3>();
            past_Left_PalmNormal = new List<Vector3>();
            past_Left_PalmDirection = new List<Vector3>();
            past_Left_Rotation = new List<Quaternion>();
            past_Left_GrabStrength = new List<float>();
            past_Left_PinchStrength = new List<float>();
            past_Left_PinchDistance = new List<float>();
            past_Left_PalmWidth = new List<float>();
            past_Left_WristPosition = new List<Vector3>();
            past_Left_Confidence = new List<float>();
            //finger
            past_Left_Finger_TipPosition = new List<List<Vector3>>();
            past_Left_Finger_Direction = new List<List<Vector3>>();            
            past_Left_Finger_Width = new List<List<float>>();               
            past_Left_Finger_Length = new List<List<float>>();            
            //bone
            past_Left_Finger_Bone_NextJoint = new List<List<List<Vector3>>>();
            past_Left_Finger_Bone_Center = new List<List<List<Vector3>>>();
            past_Left_Finger_Bone_PrevJoint = new List<List<List<Vector3>>>();
            past_Left_Finger_Bone_Direction = new List<List<List<Vector3>>>();
            past_Left_Finger_Bone_Length = new List<List<List<float>>>();
            past_Left_Finger_Bone_Width = new List<List<List<float>>>();
            past_Left_Finger_Bone_Rotation = new List<List<List<Quaternion>>>();
        }

        if (leftHand != null)
        {
            //When hands are displayed, data is stored in a list for each frame
            //↓↓↓Hand data
            leftHand_FrameNumber++;
            left_ID = leftHand.Id;
            past_Left_PalmPosition.Add(leftHand.PalmPosition.ToVector3());
            past_Left_PalmVelocity.Add(leftHand.PalmVelocity.ToVector3());
            past_Left_PalmNormal.Add(leftHand.PalmNormal.ToVector3());
            past_Left_PalmDirection.Add(leftHand.Direction.ToVector3());
            past_Left_Rotation.Add(leftHand.Rotation.ToQuaternion());
            past_Left_GrabStrength.Add(leftHand.GrabStrength);
            past_Left_PinchStrength.Add(leftHand.PinchStrength);
            past_Left_PinchDistance.Add(leftHand.PinchDistance);
            past_Left_PalmWidth.Add(leftHand.PalmWidth);
            past_Left_WristPosition.Add(leftHand.WristPosition.ToVector3());
            left_TimeVisible = leftHand.TimeVisible;
            past_Left_Confidence.Add(leftHand.Confidence);

            //Initialize the second dimension of the finger list
            past_Left_Finger_TipPosition.Add(new List<Vector3>());
            past_Left_Finger_Direction.Add(new List<Vector3>());
            past_Left_Finger_Width.Add(new List<float>());
            past_Left_Finger_Length.Add(new List<float>());

            //↓↓↓finger data
            left_Finger = leftHand.Fingers;
            for (int i = 0; i <= 4; i++)
            {
                left_FingerType[i] = leftHand.Fingers[i].Type;
                left_Finger_ID[i] = leftHand.Fingers[i].Id;
                left_Finger_HandID[i] = leftHand.Fingers[i].HandId;
                past_Left_Finger_TipPosition[leftHand_FrameNumber].Add(leftHand.Fingers[i].TipPosition.ToVector3());
                past_Left_Finger_Direction[leftHand_FrameNumber].Add(leftHand.Fingers[i].Direction.ToVector3());
                past_Left_Finger_Width[leftHand_FrameNumber].Add(leftHand.Fingers[i].Width);
                past_Left_Finger_Length[leftHand_FrameNumber].Add(leftHand.Fingers[i].Length);
                
                left_ISExtended[i] = leftHand.Fingers[i].IsExtended;
            }
            //↓↓↓bone data
            //Initialize the second dimension of the bone list
            past_Left_Finger_Bone_NextJoint.Add(new List<List<Vector3>>());
            past_Left_Finger_Bone_Center.Add(new List<List<Vector3>>());
            past_Left_Finger_Bone_PrevJoint.Add(new List<List<Vector3>>());
            past_Left_Finger_Bone_Direction.Add(new List<List<Vector3>>());
            past_Left_Finger_Bone_Length.Add(new List<List<float>>());
            past_Left_Finger_Bone_Width.Add(new List<List<float>>());
            past_Left_Finger_Bone_Rotation.Add(new List<List<Quaternion>>());
            for (int i = 0; i <= 4; i++)      //finger×5
            {
                //Initialize the third dimension of the bone list
                past_Left_Finger_Bone_NextJoint[leftHand_FrameNumber].Add(new List<Vector3>());
                past_Left_Finger_Bone_Center[leftHand_FrameNumber].Add(new List<Vector3>());
                past_Left_Finger_Bone_PrevJoint[leftHand_FrameNumber].Add(new List<Vector3>());
                past_Left_Finger_Bone_Direction[leftHand_FrameNumber].Add(new List<Vector3>());
                past_Left_Finger_Bone_Length[leftHand_FrameNumber].Add(new List<float>());
                past_Left_Finger_Bone_Width[leftHand_FrameNumber].Add(new List<float>());
                past_Left_Finger_Bone_Rotation[leftHand_FrameNumber].Add(new List<Quaternion>());
                for (int j = 0; j <= 3; j++)  //bone×4
                {
                    //get bone
                    if (j == 0)
                    {
                        left_Finger_Bone[i, j] = leftHand.Fingers[i].Bone(Bone.BoneType.TYPE_METACARPAL);
                    }
                    else if (j == 1)
                    {
                        left_Finger_Bone[i, j] = leftHand.Fingers[i].Bone(Bone.BoneType.TYPE_PROXIMAL);
                    }
                    else if (j == 2)
                    {
                        left_Finger_Bone[i, j] = leftHand.Fingers[i].Bone(Bone.BoneType.TYPE_INTERMEDIATE);
                    }
                    else if (j == 3)
                    {
                        left_Finger_Bone[i, j] = leftHand.Fingers[i].Bone(Bone.BoneType.TYPE_DISTAL);
                    }
                    //Substitute parameters for each bone from the obtained bone
                    left_Finger_Bone_Type[i, j] = left_Finger_Bone[i, j].Type;
                    past_Left_Finger_Bone_NextJoint[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].NextJoint.ToVector3());
                    past_Left_Finger_Bone_Center[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].Center.ToVector3());
                    past_Left_Finger_Bone_PrevJoint[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].PrevJoint.ToVector3());
                    past_Left_Finger_Bone_Direction[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].Direction.ToVector3());
                    past_Left_Finger_Bone_Length[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].Length);
                    past_Left_Finger_Bone_Width[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].Width);
                    past_Left_Finger_Bone_Rotation[leftHand_FrameNumber][i].Add(left_Finger_Bone[i, j].Rotation.ToQuaternion());
                }
            }   
        }   
    }
 }
  

 

   


