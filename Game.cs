using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    Common.State state = Common.State.Wait;
    Transform cam,Pointer;
    public GameObject[] objs;
    RectTransform Result;
    Text Score_text;
    GameObject obj;
    Vector3 cam_target =new Vector3(0,3,0),tap_pos=Vector3.zero;//,cam_lotate=Vector3.zero;
    int length,score=0,count=0;
    float highest = 0, theta = 0,height=13;

	// Use this for initialization
	void Start ()
    {
        length = objs.Length;
        int i = Random.Range(0, length);
        obj = Instantiate(objs[i]) as GameObject;
        obj.transform.position = new Vector3(0, 4f, 0);
        cam = GameObject.Find("Main Camera").transform;
        Pointer = GameObject.Find("Pointer").transform;
        Score_text = GameObject.Find("Score").GetComponent<Text>();
        cam.LookAt(cam_target);
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (state)
        {
            case Common.State.Wait:
                Waiting();break;
            case Common.State.Select:
                Selecting();break;
            case Common.State.Fall:
                Falling();break;
            case Common.State.Out:
                Out();break;
        }	
	}

    void Waiting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.gameObject.tag == "Pointer")
            {
                state = Common.State.Select;
            }
            else
            {
                tap_pos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButton(0)&&state==Common.State.Wait)
        {
            Vector3 vec = Input.mousePosition-tap_pos;
            cam.position = new Vector3(27 * Mathf.Cos(theta - vec.x * 0.05f), Mathf.Clamp(height - vec.y*0.1f, 0, highest + 30f), 25 * Mathf.Sin(theta - vec.x * 0.05f));
            cam.LookAt(cam_target);
        }
        if (Input.GetMouseButtonUp(0) && state == Common.State.Wait)
        {
            Vector3 vec = Input.mousePosition - tap_pos;
            theta = theta - vec.x * 0.05f;
            height = Mathf.Clamp(height - vec.y * 0.1f, 0, highest + 30f);
        }
    }
    void Selecting()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Pointer") obj.transform.position = hit.point;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            obj.GetComponent<Rigidbody>().useGravity = true;
            state = Common.State.Fall;
        }
    }
    void Falling()
    {
        if (obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f) count++;
        else count = 0;
        if (count > 50)
        {
            highest = Mathf.Max(highest, obj.transform.position.y);
            Pointer.position = new Vector3(0, highest + 4, 0);
            int i = Random.Range(0, length);
            obj = Instantiate(objs[i]) as GameObject;
            obj.transform.position = new Vector3(0, highest + 4f, 0);
            state = Common.State.Wait;
            score++;
            Score_text.text = "Score\n" + score;
            count = 0;
        }
    }
    void Out()
    {
        Vector3 vec = Result.position;
        Result.position = vec * 0.9f+new Vector3(Screen.width,Screen.height)*0.05f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state!=Common.State.Out&&collision.gameObject.tag == "Object")
        {
            state = Common.State.Out;
            Result = GameObject.Find("Result").GetComponent<RectTransform>();
            Destroy(Score_text.gameObject);
            GameObject.Find("RScore").GetComponent<Text>().text = "Score : " + score;
        }
    }

    public void View(float delta)
    {
        cam_target.y = Mathf.Clamp(cam_target.y + delta, 0, highest + 8f);
        cam.LookAt(cam_target);
    }
    public void Muve(int num)
    {
        if (num == 0) obj.transform.Rotate(new Vector3(0, 1, 0), 30, Space.World);
        else if (num == 1) obj.transform.Rotate(obj.transform.position - cam.position, -30, Space.World);
        else if (num == 2) obj.transform.Rotate(Vector3.Cross(new Vector3(0, 1, 0), obj.transform.position - cam.position), 30, Space.World);
    }
    public void ReStart()
    {
        SceneManager.LoadScene("Game");
    }
}
