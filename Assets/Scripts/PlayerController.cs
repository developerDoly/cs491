using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class PlayerController : MonoBehaviour
{
    //******************************************************************
    //
    //
    private SerialPort serial;

    private string UNO_port = "/dev/cu.SLAB_USBtoUART";
    
    //grab a reference to the Rigid Body
    private new Rigidbody2D rigidbody;

    [SerializeField] private Vector2 velocity = new Vector2(0,0);
    private Vector2 oldVelocity = new Vector2(0,0);
    private float weight = .70f;//weight of newest velocity for low pass filter
    
    private float firstToken;
    private float secondToken;

    private int button_state;//1 if pushed and 0 if not pushed

    private float xMove;
    private float yMove;

    //private float movementSpeed = 1;
    //
    //
    //******************************************************************

    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        serial = new SerialPort(UNO_port, 9600); 
        serial.Open();
        
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    
    
    
    
    

    // Update is called once per frame
    void Update()
    {
        if (serial.BytesToRead > 0) 
        {
            string message = serial.ReadLine();
        
            getTokens(message);
            
            xMove = firstToken;
            yMove = secondToken;
            
            //doMovementByChangingPosition(xMove, yMove);
            doMovementByChangingVelocity(xMove, yMove, oldVelocity, 100);


            Debug.Log("message from arduino: " + message);
        }
    }
    
    
    
    
    
    
    
    
    //Strings are immutable, so simply adding to a string will create many intermediate temporary string allocations on the heap
    //this is inefficient, so instead we will use String.Create to fill in character content via a callback 
    //i.e. we will mutate the string!!!!!
    //This is much more performant, but dangerous so we have to careful and we should test for all cases.
    void FastRead()
    {
        /*
        
        // constructing a string from a char array, prefix it with some additional characters
        char[] chars = { 'a', 'b', 'c', 'd', '\0' };
        int length = chars.Length + 2;
        string result = string.Create (length, chars, (Span<char> strContent, char[] charArray) =>
        {
            strContent[0] = '0';
            strContent[1] = '1';
            for (int i = 0; i < charArray.Length; i++)
            {
                strContent[i + 2] = charArray[i];
            }
        });

        Debug.Log(result);
        */
        
    }
    
    
    
    
    
    
    
    
    

    void doMovementByChangingVelocity(float xMove, float yMove, Vector2 oldVelocity_local, int scale) 
    {
        oldVelocity = new Vector2(xMove/scale, yMove/scale) ;// SOME_SCALE_FACTOR;\
        //low pass filter
        Vector2 oldVelocityWeighted = new Vector2(oldVelocity.x * weight, oldVelocity.y * weight);
        float weight_inverse = 1 - weight;
        Vector2 oldVelocity_localWeighted = new Vector2(oldVelocity_local.x * weight, oldVelocity_local.y * weight);

        rigidbody.velocity = new Vector2(oldVelocity_localWeighted.x + oldVelocityWeighted.x, oldVelocity_localWeighted.y + oldVelocityWeighted.y);
    } 
    
    
    
    
    
    
    
    
    
    
    
    

    void doMovementByChangingPosition(float xMove, float yMove) 
    {
        //do the movement
        //transform.position += new Vector3(xMove, yMove, 0) / 1000f;
    }
    
    
    
    
    
    
    
    
    
    
    
    

    
    void getTokens(string message)
    {
        string[] tokens = message.Split(' ');// [0 0]
        //grab first token

        string firststep = tokens[0].Substring(1);
        
        string secondstep = tokens[1];

        string button_state_local = tokens[2].Substring(0, tokens[2].Length - 2);


        Debug.Log("first step: " + firststep);
        Debug.Log("second step: " + secondstep);
        Debug.Log("button pushed: " + button_state_local);
        
        //firstToken = float.Parse(tokens[0].Substring(1));//cuts off the first bracket
        firstToken = float.Parse(firststep);

        //grab second token
      
        // secondToken = float.Parse(tokens[1].Substring(0, tokens[1].Length - 2));//cuts off the last bracket
        secondToken = float.Parse(secondstep);
       
        button_state = int.Parse(button_state_local);
    }
    
    
    
    
    
    
    
    
    
    
    
    

    void OnTriggerEnter(Collider other)
    {
        Destroy(other);
    }
    
}
