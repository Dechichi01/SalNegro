using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MobileInputManager : MonoBehaviour {

    //Assigned in the inspector
    public TouchInput[] touchInputs;
    public bool enableSwipe;
    public float minSwipeDistY = 0f;
    public float minSwipeDistX = 0f;

    //
    private SwipeDirection swipeDirection;
    private Vector2 startPos;
    private float starTime;

    List<Touch> frameTouches;

    private void Update()
    {
        //Clear all pressed buttons
        for (int i = 0; i < touchInputs.Length; i++)
            touchInputs[i].pressed = false;

        //Chefk for touch inputs
        if (Input.touchCount > 0)
        {
            frameTouches = new List<Touch>(Input.touches);

            //Check for touches in determined areas
            for (int i = 0; i<frameTouches.Count; i++)
            {
                for (int j = 0; j < touchInputs.Length; j++)
                {
                    //Will set as a pressed touch if it's inside the touch area and the area is touched or holded (for holdable touchs only)
                    if (
                        (frameTouches[i].phase == TouchPhase.Began || touchInputs[j].holdable) 
                        && RectTransformUtility.RectangleContainsScreenPoint(touchInputs[j].area, frameTouches[i].position))
                    {
                        touchInputs[j].pressed = true;
                        frameTouches.RemoveAt(i);
                        i--;//Decrement i so won't jump a element in the next iteration
                        break;//Break the inner loop
                    }
                }
            }

            //Check for swipe
            if (enableSwipe && frameTouches.Count > 0)
            {
                Touch touch = frameTouches[0];

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPos = touch.position;
                        starTime = Time.time;
                        break;
                    case TouchPhase.Ended:
                        float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                        if (swipeDistVertical > minSwipeDistY)
                        {
                            float swipeValue = Mathf.Sign(touch.position.y - startPos.y);
                            if (swipeValue > 0)
                                swipeDirection = SwipeDirection.Up;

                            else if (swipeValue < 0)//down swipe
                                swipeDirection = SwipeDirection.Down;
                        }
                        float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;
                        if (swipeDistHorizontal > minSwipeDistX)
                        {
                            float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
                            if (swipeValue > 0)//right swipe
                                swipeDirection = SwipeDirection.Right;
                            else if (swipeValue < 0)//left swipe
                                swipeDirection = SwipeDirection.Left;
                        }
                        break;
                }
            }
        }
    }

    public SwipeDirection GetSwipeDirection() // to be used by Update()
    {
        if (swipeDirection != SwipeDirection.Null)//if a swipe is detected
        {
            SwipeDirection etempSwipeDirection = swipeDirection;
            swipeDirection = SwipeDirection.Null;
            return etempSwipeDirection;
        }
        else
        {
            return SwipeDirection.Null;//if no swipe was detected
        }
    }

    [System.Serializable]
    public struct TouchInput
    {
        public RectTransform area;
        public bool pressed;
        public ActionType actionType;
        public bool holdable;
    }

    public enum SwipeDirection
    {
        Null = 0, //no swipe detected
        Down = 1, //swipe down detected
        Up = 2, //swipe up detected
        Right = 3, //swipe right detected
        Left = 4, //swipe left detected
    }
}

public enum ActionType
{
    Attack,
    Jump,
    Roll,
    MoveRight,
    MoveLeft
}
