#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public static class StreetFootballBootstrap
{
    [MenuItem("Street Football/Build Demo Scene")]
    public static void BuildDemo(){
        Directory.CreateDirectory("Assets/Scenes");
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        RenderSettings.ambientLight=Color.gray*.65f;
        RenderSettings.fog=true; RenderSettings.fogColor=new Color(.48f,.48f,.48f); RenderSettings.fogDensity=.012f;
        MakeLight(); MakePitch(); MakePlayer(); MakeBall(); MakeTeams(); MakeCamera(); MakeUI();
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/StreetMatch.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/StreetMatch.unity",true)};
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        Debug.Log("Street Football demo scene generated.");
    }
    static GameObject Cube(string n,Vector3 p,Vector3 s,Color c){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.position=p;g.transform.localScale=s;g.GetComponent<Renderer>().material=Mat(c);return g;}
    static Material Mat(Color c){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=c;return m;}
    static void MakeLight(){var g=new GameObject("Sun");var l=g.AddComponent<Light>();l.type=LightType.Directional;l.intensity=1.3f;g.transform.rotation=Quaternion.Euler(48,-30,0);}
    static void MakePitch(){
        Cube("Street",Vector3.zero,new Vector3(28,.15f,18),new Color(.12f,.13f,.14f));
        Cube("Pitch",new Vector3(0,.1f,0),new Vector3(24,.08f,14),new Color(.08f,.36f,.18f));
        foreach(float x in new[]{-12f,12f}){Cube("Touchline",new Vector3(x,.17f,0),new Vector3(.08f,.03f,14),Color.white);}
        foreach(float z in new[]{-7f,7f})Cube("Touchline",new Vector3(0,.17f,z),new Vector3(24,.03f,.08f),Color.white);
        Cube("Halfway",new Vector3(0,.17f,0),new Vector3(.04f,.03f,14),Color.white);
        Cube("GoalBlue",new Vector3(11.8f,1.1f,0),new Vector3(.25f,2.2f,4),Color.white);
        Cube("GoalRed",new Vector3(-11.8f,1.1f,0),new Vector3(.25f,2.2f,4),Color.white);
        for(int i=-2;i<=2;i++){Cube("Wall",new Vector3(0,1.2f,9),new Vector3(5,2.4f,.35f),new Color(.45f,.35f,.25f));}
        for(int i=0;i<5;i++){float z=-7+i*3.5f;Cube("StreetBuilding",new Vector3(-15,2,z),new Vector3(2,4,3),new Color(.25f,.22f,.2f));Cube("StreetBuilding",new Vector3(15,2,z),new Vector3(2,4,3),new Color(.28f,.24f,.2f));}
    }
    static void MakePlayer(){var g=GameObject.CreatePrimitive(PrimitiveType.Capsule);g.name="Pakistan_Player";g.tag="Player";g.transform.position=new Vector3(-5,1,0);g.transform.localScale=new Vector3(.7f,1.1f,.7f);g.GetComponent<Renderer>().material=Mat(new Color(.04f,.25f,.75f));g.AddComponent<Rigidbody>().constraints=RigidbodyConstraints.FreezeRotation;}
    static void MakeBall(){var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name="Ball";g.transform.position=new Vector3(-3,.45f,0);g.transform.localScale=Vector3.one*.65f;g.GetComponent<Renderer>().material=Mat(Color.white);var rb=g.AddComponent<Rigidbody>();rb.mass=.45f;rb.linearDamping=.4f;rb.angularDamping=.2f;}
    static void MakeTeams(){for(int i=0;i<3;i++)MakeAI("Blue_AI_"+i,new Vector3(-2+i*1.2f,1,-4+i*4),true,new Color(.04f,.25f,.75f));for(int i=0;i<3;i++)MakeAI("Red_AI_"+i,new Vector3(4+i*1.2f,1,-4+i*4),false,new Color(.75f,.05f,.05f));}
    static void MakeAI(string n,Vector3 p,bool blue,Color c){var g=GameObject.CreatePrimitive(PrimitiveType.Capsule);g.name=n;g.transform.position=p;g.transform.localScale=new Vector3(.65f,1,.65f);g.GetComponent<Renderer>().material=Mat(c);g.AddComponent<SimpleFootballAI>().blueTeam=blue;}
    static void MakeCamera(){var g=new GameObject("BroadcastCamera");g.transform.position=new Vector3(-5,9,-13);var c=g.AddComponent<Camera>();c.fieldOfView=48;var bc=g.AddComponent<BroadcastCamera>();bc.target=GameObject.FindWithTag("Player").transform;}
    static void MakeUI(){
        var ev=new GameObject("EventSystem");ev.AddComponent<EventSystem>();ev.AddComponent<StandaloneInputModule>();
        var canvas=new GameObject("HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var cv=canvas.GetComponent<Canvas>();cv.renderMode=RenderMode.ScreenSpaceOverlay;canvas.GetComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;canvas.GetComponent<CanvasScaler>().referenceResolution=new Vector2(1920,1080);
        var game=GameObject.FindObjectOfType<StreetFootballGame>();if(!game)game=new GameObject("GameManager").AddComponent<StreetFootballGame>();
        game.controlledPlayer=GameObject.FindWithTag("Player").transform;game.ball=GameObject.Find("Ball").GetComponent<Rigidbody>();
        game.scoreText=Text("Score","PAKISTAN  0  -  0  OPPONENT",canvas.transform,new Vector2(0,1),new Vector2(0,1),new Vector2(0,0),new Vector2(900,80),36);game.timeText=Text("Time","03:00",canvas.transform,new Vector2(1,1),new Vector2(1,1),new Vector2(0,0),new Vector2(260,80),40);game.messageText=Text("Message","",canvas.transform,new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(0,0),new Vector2(800,70),30);
        var joy=UIObj("Joystick",canvas.transform,new Vector2(0,0),new Vector2(0,0),new Vector2(60,70),new Vector2(300,300));var bg=joy.AddComponent<Image>();bg.color=new Color(1,1,1,.16f);var v=joy.AddComponent<VirtualJoystick>();v.area=joy.GetComponent<RectTransform>();game.joystick=v;var knob=UIObj("Knob",joy.transform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),Vector2.zero,new Vector2(120,120));knob.AddComponent<Image>().color=new Color(1,1,1,.45f);v.knob=knob.GetComponent<RectTransform>();
        Button(canvas.transform,"SHOOT",new Vector2(1,0),new Vector2(1,0),new Vector2(-210,170),new Vector2(190,110),()=>game.Shoot());Button(canvas.transform,"PASS",new Vector2(1,0),new Vector2(1,0),new Vector2(-430,105),new Vector2(170,90),()=>game.Pass());Button(canvas.transform,"TACKLE",new Vector2(1,0),new Vector2(1,0),new Vector2(-420,230),new Vector2(170,90),()=>game.Tackle());Button(canvas.transform,"SPRINT",new Vector2(0,0),new Vector2(0,0),new Vector2(310,105),new Vector2(180,90),()=>game.SprintDown());
    }
    static GameObject UIObj(string n,Transform p,Vector2 amin,Vector2 amax,Vector2 pos,Vector2 size){var g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);var r=g.GetComponent<RectTransform>();r.anchorMin=amin;r.anchorMax=amax;r.anchoredPosition=pos;r.sizeDelta=size;return g;}
    static Text Text(string n,string s,Transform p,Vector2 amin,Vector2 amax,Vector2 pos,Vector2 size,int fs){var g=UIObj(n,p,amin,amax,pos,size);var t=g.AddComponent<Text>();t.text=s;t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=fs;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return t;}
    static void Button(Transform p,string label,Vector2 amin,Vector2 amax,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction a){var g=UIObj(label,p,amin,amax,pos,size);var img=g.AddComponent<Image>();img.color=new Color(.03f,.08f,.12f,.75f);var b=g.AddComponent<Button>();b.onClick.AddListener(a);Text("Label",label,g.transform,new Vector2(0,0),new Vector2(1,1),Vector2.zero,Vector2.zero,28);}
}
#endif
