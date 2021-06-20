using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playermovement : MonoBehaviour
{
    public float moveForce = 0f;
    private Rigidbody rb;
    public int HP = 0;
    //public Slider HPBar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal") * moveForce;
        float v = Input.GetAxisRaw("Vertical") * moveForce;

        rb.velocity = transform.TransformVector(h, 0, v);
      //  HPBar.value = HP;
    }
}
