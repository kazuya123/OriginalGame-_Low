using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public float jumpForce;
    public float moveSpeed;
    public float sphereRadius;
    public LayerMask mask;
    public GameObject hitObject;
    public GameObject myCentral;

    private Animator  _animator;
    private Rigidbody _rigid;
    private int   _attack= 0;
    private int   _hit = 0;
    private bool _isGround = false;
    private bool _isJump = false;
    private bool _isMove = false; 
    private bool _isDamege = false;

    // Use this for initialization
    void Start () {
        _rigid = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        _isGround = GroundCheck();

        //_animator.SetBool("Move", false);
        //_animator.SetInteger("Attack", 0);
        //_isMove = false;

        //Debug.Log(_isGround + "  接地判定");
        //Debug.Log(_isDamege + "  無敵判定");
        //Debug.Log(_hit + "  攻撃判定");

        //Debug.Log(_jumpTopHeight + "  ジャンプの高さ");
        //Debug.Log("param : " + _animator.GetInteger("Jump"));

        /*
        //移動------------------------------------------------------------------------------
        if (Input.GetKey(KeyCode.LeftArrow) && _isGround && !_isDamege)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _animator.SetBool("Move", true);
            _isMove = true;
        }

        if (Input.GetKey(KeyCode.RightArrow) && _isGround && !_isDamege)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.SetBool("Move", true);
            _isMove = true;
        }

        //攻撃------------------------------------------------------------------------------
        if (Input.GetKey(KeyCode.Space) && _attack == 0 && _isGround && !_isMove)
        {
            _animator.SetInteger("Attack", 1);
        }

        if (Input.GetKey(KeyCode.Space) && _attack == 1 && _isGround && !_isMove)
        {
            _animator.SetInteger("Attack", 2);
        }

        if (Input.GetKey(KeyCode.Space) && _attack == 2 && _isGround && !_isMove)
        {
            _animator.SetInteger("Attack", 3);
        }

        //ジャンプ------------------------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.UpArrow) && _isGround)
        {
            _isJump = true;
            _animator.SetInteger("Jump", 1);
        }

        if (Input.GetKey(KeyCode.UpArrow) && _currentSecond <= _jumpSecond && _isJump)
        {
            _slowdownForce += Time.deltaTime * 400;
            _currentSecond += Time.deltaTime;
            _rigid.AddForce(Vector3.up * (jumpForce - _slowdownForce));

            //Debug.Log("上昇中");
            // Debug.Log(jumpForce - _slowdownForce);
            _animator.SetInteger("Jump", 2);
        }

        if (_jumpTopHeight >= transform.position.y && !_isGround)
        {
            //Debug.Log("下降中");
            _animator.SetInteger("Jump", 4);
        }

        _jumpTopHeight = Mathf.Max(_jumpTopHeight, transform.position.y);
            */
    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, sphereRadius, mask);
    }

    IEnumerator DamegeCheck()
    {
        yield return new WaitForSeconds(0.5f);
         _isDamege = false;   
    }

    void FixedUpdate()
    {

    }

    //攻撃判定------------------------------------------------------------------------------
    void NewEvent(int state)
    {
        _attack = state;
    }

    void NewEvent2()
    {
        _attack = 0;
        _animator.SetInteger("Attack", 0);
    }

    void HitCheck(int state)
    {
        _hit = state;
    }

    //接地判定------------------------------------------------------------------------------
    void OnCollisionEnter(Collision other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);

        if(layerName == "Ground")
        {
            _isJump = false;
            _animator.SetInteger("Jump", 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerWeapon"))
        {
            var vec1 = other.gameObject.transform.position;
            var vec2 = myCentral.transform.position;
            var res = vec1 - vec2;
            Debug.Log(res.ToString() + "   結果 ");

            _rigid.velocity = Vector3.zero;
            _animator.SetBool("Damege", true);

            _rigid.AddForce(-res * 400);

            _isDamege = true;
            StartCoroutine("DamegeCheck");
        }
    }
}
