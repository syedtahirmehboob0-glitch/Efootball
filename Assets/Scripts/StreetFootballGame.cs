using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StreetFootballGame : MonoBehaviour
{
    public static StreetFootballGame I;
    public Transform controlledPlayer;
    public Rigidbody ball;
    public Text scoreText, timeText, messageText;
    public VirtualJoystick joystick;
    public Camera matchCamera;
    int blueScore, redScore;
    float matchTime = 180f;
    bool shooting, sprinting;
    readonly List<SimpleFootballAI> players = new();

    void Awake(){ I=this; }
    void Start(){
        if(!controlledPlayer) controlledPlayer=GameObject.FindWithTag("Player")?.transform;
        if(!ball){ var b=GameObject.Find("Ball"); if(b) ball=b.GetComponent<Rigidbody>(); }
        RefreshHUD();
        messageText.text="PAKISTAN STREET FOOTBALL";
    }
    void Update(){
        matchTime=Mathf.Max(0,matchTime-Time.deltaTime);
        if(timeText) timeText.text=$"{Mathf.FloorToInt(matchTime/60):00}:{Mathf.FloorToInt(matchTime%60):00}";
        if(matchTime<=0){ messageText.text="FULL TIME"; enabled=false; }
        if(controlledPlayer){
            Vector2 input=joystick?joystick.Value:new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
            float speed=sprinting?7f:4.5f;
            Vector3 move=new Vector3(input.x,0,input.y);
            if(move.sqrMagnitude>.02f){
                controlledPlayer.position+=move.normalized*speed*Time.deltaTime;
                controlledPlayer.forward=Vector3.Slerp(controlledPlayer.forward,move.normalized,12f*Time.deltaTime);
            }
            if(shooting) TryKick(15f); shooting=false;
        }
        sprinting=Input.GetKey(KeyCode.LeftShift);
    }
    public void Shoot(){ shooting=true; }
    public void SprintDown(){ sprinting=true; }
    public void SprintUp(){ sprinting=false; }
    public void Pass(){ TryKick(7f); }
    public void Tackle(){
        if(!controlledPlayer)return;
        foreach(var ai in players){ if(ai && Vector3.Distance(controlledPlayer.position,ai.transform.position)<2f) ai.Stun(0.5f); }
    }
    void TryKick(float power){
        if(!ball||!controlledPlayer)return;
        Vector3 p=controlledPlayer.position+controlledPlayer.forward*1.15f+Vector3.up*.25f;
        if(Vector3.Distance(ball.position,p)<2.2f){
            ball.position=p; ball.velocity=controlledPlayer.forward*power+Vector3.up*(power>.9f?1.4f:.4f);
        }
    }
    public void Register(SimpleFootballAI p){if(!players.Contains(p))players.Add(p);}
    public void Goal(bool blue){if(blue)blueScore++;else redScore++; RefreshHUD(); ResetAfterGoal();}
    void RefreshHUD(){if(scoreText)scoreText.text=$"PAKISTAN  {blueScore}  -  {redScore}  OPPONENT";}
    void ResetAfterGoal(){if(ball){ball.position=Vector3.up*.35f;ball.velocity=Vector3.zero;}}
}

public class SimpleFootballAI:MonoBehaviour
{
    public bool blueTeam; public Transform target; public float speed=3.2f; float stun;
    Rigidbody rb;
    void Start(){rb=GetComponent<Rigidbody>(); if(StreetFootballGame.I)StreetFootballGame.I.Register(this);}
    public void Stun(float t){stun=t;}
    void Update(){
        if(stun>0){stun-=Time.deltaTime;return;}
        if(!target){var b=GameObject.Find("Ball");if(b)target=b.transform;}
        if(!target)return;
        Vector3 goal=new Vector3(blueTeam?10:-10,transform.position.y,0);
        Vector3 desired=Vector3.Distance(transform.position,target.position)<7?target.position:Vector3.Lerp(target.position,goal,.35f);
        Vector3 d=desired-transform.position; d.y=0;
        if(d.sqrMagnitude>.3f){transform.position+=d.normalized*speed*Time.deltaTime;transform.forward=Vector3.Slerp(transform.forward,d.normalized,8*Time.deltaTime);}
    }
}

public class VirtualJoystick:MonoBehaviour, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler
{
    public RectTransform area, knob; public Vector2 Value{get;private set;}
    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData e){OnDrag(e);}
    public void OnDrag(UnityEngine.EventSystems.PointerEventData e){
        if(!area)area=(RectTransform)transform; Vector2 local; RectTransformUtility.ScreenPointToLocalPointInRectangle(area,e.position,e.pressEventCamera,out local);
        float r=Mathf.Min(area.rect.width,area.rect.height)*.5f; Value=Vector2.ClampMagnitude(local/r,1); if(knob)knob.anchoredPosition=Value*r*.65f;
    }
    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData e){Value=Vector2.zero;if(knob)knob.anchoredPosition=Vector2.zero;}
}

public class BroadcastCamera:MonoBehaviour
{
    public Transform target; public Vector3 offset=new Vector3(0,9,-13); public float smooth=6;
    void LateUpdate(){if(!target)return;Vector3 desired=target.position+offset;transform.position=Vector3.Lerp(transform.position,desired,smooth*Time.deltaTime);transform.LookAt(target.position+Vector3.up*.8f);}
}
