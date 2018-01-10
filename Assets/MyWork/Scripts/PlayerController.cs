using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float jumpForce;  　　　//ジャンプ力 プレイヤー専用
    public float moveSpeed;　　　　//移動速度
    public float sphereRadius;　 　//CheckSphere用　半径
    public LayerMask mask;　　　 　//CheckSphere用　検知レイヤー
    public GameObject hitObject;　 //
    public GameObject myCentral; 　//自機の中心座標
    public GameObject HPBar;　　　 //HPバー プレイヤー専用
    public MeshRenderer[] model;          //無敵時点滅モデル
    public SkinnedMeshRenderer[] model2;  //無敵時点滅モデル
    public bool _isInvisible = false;     //無敵判定

    private Animator  _animator;
    private Rigidbody _rigid;
    private int   _attack= 0;
    private int   _hit = 0;
    private int   _jump = 0;
    private int   _attackCount = 0;
    private float _jumpTopHeight;　　　//ジャンプの最高地点 プレイヤー専用
    private float _jumpSecond = 0.2f;　//ジャンプの入力受付 プレイヤー専用
    private float _currentSecond;      //ジャンプの時間経過 プレイヤー専用
    private float _slowdownForce;　　　//ジャンプの減速用　 プレイヤー専用
    private float _horizontalInput;　　//左右キー入力受付　 プレイヤー専用
    private bool _isGround = false;
    private bool _isDamege = false; 


    private float leftPushedTime = -10.0f; //左キーが押された時間
    private float rightPushedTime = -10.0f; //右キーが押された時間
    private static float DASH_SENSE_DURATION = 0.5f; //キーを２回押した時にダッシュとみなす時間差(秒)
    private static float DASH_SPEED_SCALE = 1.5f; //ダッシュ中の速度スケール値
    private bool dashRight; //ダッシュ中か
    private bool dashLeft; //ダッシュ中か


    // Use this for initialization
    void Start () {
        _rigid = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        _horizontalInput = Input.GetAxis("Horizontal");
        //_horizontalInput *= 0.99f;

        _isGround = GroundCheck();

        if (!_isGround) _animator.SetBool("Air", true);
        if (_isGround) _animator.SetBool("Air", false);

        if (_isInvisible)
        {
            float level = Mathf.Abs(Mathf.Sin(Time.time * 8) * 0.7f) + 0.7f;
            model[0].material.color = new Color(level, level, level, 0f);
            model2[0].material.color = new Color(level, level, level, 0f);
            model2[1].material.color = new Color(level, level, level, 0f);
            model2[2].material.color = new Color(level, level, level, 0f);
        }

        if (!_isInvisible)
        {
            model[0].material.color = new Color(1f, 1f, 1f, 0f);
            model2[0].material.color = new Color(1f, 1f, 1f, 0f);
            model2[1].material.color = new Color(1f, 1f, 1f, 0f);
            model2[2].material.color = new Color(1f, 1f, 1f, 0f);
        }


        _animator.SetBool("Move", false);
        _animator.SetBool("Run", false);

        Debug.Log(_attackCount + "攻撃回数");
        //Debug.Log(_isGround + "  接地判定");
        //Debug.Log(_attack + "攻撃種類");
        Debug.Log(_isInvisible + "  無敵判定");
        //Debug.Log(_hit + "  攻撃判定");
        //Debug.Log(_jumpTopHeight + "  ジャンプの高さ");
        //Debug.Log("param : " + _animator.GetInteger("Jump"));


//ニュートラル攻撃------------------------------------------------------------------------------
        //初段攻撃（単発）
        if (Input.GetKeyDown(KeyCode.Space) && _attack == 0 && _isGround && !_animator.GetBool("Move"))
        {
            _animator.SetInteger("Attack", 1);
            _animator.SetBool("isAttack", true);
        }
        //二段攻撃（ループ）
        if (Input.GetKey(KeyCode.Space) && _attack == 1 && _isGround && !_animator.GetBool("Move"))
        {
            _animator.SetInteger("Attack", 2);
        }
        //三段攻撃（ループ）
        if (Input.GetKey(KeyCode.Space) && _attack == 2 && _isGround && !_animator.GetBool("Move"))
        {
            _animator.SetInteger("Attack", 3);
        }
        //空中攻撃
        if (Input.GetKeyDown(KeyCode.Space) && _animator.GetBool("Air") )
        {
            _animator.SetBool("AirAttack", true);
        }
        //ダッシュ攻撃
        if (Input.GetKeyDown(KeyCode.Space) && _animator.GetBool("Run") && _isGround)
        {
            Debug.Break();
            _animator.SetInteger("Attack", 20);
        }

        //方向入力攻撃------------------------------------------------------------------------------
        if (Input.GetKeyUp(KeyCode.Space)  && _isGround && !_animator.GetBool("Move") && _attackCount >= 3)
        {
            if (this.transform.rotation.eulerAngles.y > 90)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Debug.Log("左向き前");
                    _animator.SetInteger("Attack", 10);
                    StartCoroutine("Attack_F");
                }

                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    Debug.Log("左向き後");
                    _animator.SetInteger("Attack", 11);
                    StartCoroutine("Attack_B");
                }

                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    Debug.Log("左向き上");
                    _animator.SetInteger("Attack", 12);
                    StartCoroutine("Attack_U");
                }

                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    Debug.Log("左向き下");
                    _animator.SetInteger("Attack", 13);
                }

            }

            else if (this.transform.rotation.eulerAngles.y < 90)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Debug.Log("右向き前");
                    _animator.SetInteger("Attack", 10);
                    StartCoroutine("Attack_F");
                }

                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Debug.Log("右向き後");
                    _animator.SetInteger("Attack", 11);
                    StartCoroutine("Attack_B");
                }

                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    Debug.Log("右向き上");
                    _animator.SetInteger("Attack", 12);
                    StartCoroutine("Attack_U");
                }

                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    Debug.Log("右向き下");
                    _animator.SetInteger("Attack", 13);
                }
            }

        }

        //移動（入力、向き、モーション切り替え）------------------------------------------------------------------------------

        //キーが押された瞬間
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //ダッシュが必要ならtrueが入る
            dashLeft = Time.time - leftPushedTime < DASH_SENSE_DURATION;
            //キーが押された現在の時間を格納
            leftPushedTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //ダッシュが必要ならtrueが入る
            dashRight = Time.time - rightPushedTime < DASH_SENSE_DURATION;
            //キーが押された現在の時間を格納
            rightPushedTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            dashRight = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            dashLeft = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow) && !_isDamege && !_animator.GetBool("isAttack") && !_animator.GetBool("Dead"))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);

            if (!_animator.GetBool("isAttack") && _isGround)
            {
                //ダッシュ中ならダッシュ専用アニメ
                if (dashLeft || dashRight)
                {
                    _animator.SetBool("Run", true);
                }
                else
                {
                    _animator.SetBool("Move", true);
                }
            }
        }

        if (Input.GetKey(KeyCode.RightArrow) && !_isDamege && !_animator.GetBool("isAttack") && !_animator.GetBool("Dead"))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);

            if (!_animator.GetBool("isAttack") && _isGround)
            {
                //ダッシュ中ならダッシュ専用アニメ
                if (dashLeft || dashRight)
                {
                    _animator.SetBool("Run", true);
                }
                else
                {
                    _animator.SetBool("Move", true);
                }
            }
        }

//ジャンプ------------------------------------------------------------------------------

        //ジャンプ入力
        if (Input.GetKeyDown(KeyCode.UpArrow) && _isGround && !_animator.GetBool("isAttack"))
        {
            _animator.SetInteger("Jump", 1);
            _attack = 0;
            _animator.SetInteger("Attack", 0);
        }

        //ジャンプ入力中処理
        if (Input.GetKey(KeyCode.UpArrow) && _currentSecond <= _jumpSecond  && _jump == 2 && !_animator.GetBool("isAttack"))
        {
            if (jumpForce >= _slowdownForce)
            {
                _slowdownForce += Time.deltaTime * 1400;
            }

            _currentSecond += Time.deltaTime;
            _rigid.AddForce(Vector3.up * (jumpForce - _slowdownForce));

            Debug.Log("上昇中");
            _animator.SetInteger("Jump", 2);
        }

        //ジャンプ入力が短かった場合
        if (Input.GetKeyUp(KeyCode.UpArrow) && _isGround)
        {
            _animator.SetInteger("Jump", 0);
        }

        //空中ジャンプ
        if (Input.GetKeyDown(KeyCode.UpArrow) && _animator.GetBool("Air") && !_animator.GetBool("useAirJump"))
        {
            _animator.SetBool("AirJump", true);

            _rigid.velocity = Vector3.zero;
            _rigid.AddForce(Vector3.up * (jumpForce * 3));

            _animator.SetBool("useAirJump", true);
        }

        //落下中
        if (_jumpTopHeight >= transform.position.y && !_isGround)
        {
            Debug.Log("下降中");
            _animator.SetInteger("Jump", 3);
        }

        _jumpTopHeight = Mathf.Max(_jumpTopHeight, transform.position.y);

    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, sphereRadius, mask);
    }

//遅延処理------------------------------------------------------------------------------
    IEnumerator DamegeCheck()
    {
        yield return new WaitForSeconds(0.5f);
        _isDamege = false;
        _attack = 0;
        _animator.SetInteger("Attack", 0);

        yield return new WaitForSeconds(1f);
        _isInvisible = false;
    }

    IEnumerator Attack_U()
    {
        yield return new WaitForSeconds(0.3f);
        _rigid.AddForce(Vector3.up * 500);
    }

    IEnumerator Attack_F()
    {
        yield return new WaitForSeconds(0.25f);
        _rigid.velocity = new Vector3(moveSpeed * 2 * _horizontalInput, _rigid.velocity.y + 1, _rigid.velocity.z);
    }

    IEnumerator Attack_B()
    {
        yield return new WaitForSeconds(0.25f);

        if (this.transform.rotation.eulerAngles.y > 90) {
            _rigid.velocity = new Vector3(moveSpeed * 2, _rigid.velocity.y + 3, _rigid.velocity.z);
        }
        if (this.transform.rotation.eulerAngles.y < 90)
        {
            _rigid.velocity = new Vector3(moveSpeed * -2, _rigid.velocity.y + 3, _rigid.velocity.z);
        }

    }

//アニメーションイベント用-----------------------------------------------------------------------------
    void NewEvent(int state)
    {
        _attack = state;
        _jump = state;
        _attackCount++;
    }

    void NewEvent2()
    {
        _attack = 0;
        _animator.SetBool("isAttack", false);
        _animator.SetInteger("Attack", 0);
        _attackCount =0;
    }

    void HitCheck(int state)
    {
        _hit = state;
    }

//接地、衝突判定------------------------------------------------------------------------------
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _currentSecond = 0;
            _slowdownForce = 0;
            _jumpTopHeight = _rigid.transform.position.y;
            _jump = 0;
            _animator.SetInteger("Jump", 4);
            _attack = 0;
            _animator.SetBool("useAirJump", false);
        }

        if (other.gameObject.CompareTag("Enemy") && !_isInvisible)
        {
            _isDamege = true;
            _isInvisible = true;

            ContactPoint contact = other.contacts[0];
            Vector3 contactPos = contact.point;

            var vec1 = contactPos;
            var vec2 = myCentral.transform.position;
            var res = vec1 - vec2;

            _rigid.velocity = Vector3.zero;
            _rigid.AddForce(-res * 600);




                _animator.SetBool("Damege", true);




            StartCoroutine("DamegeCheck");

        }
    }

    void FixedUpdate()
    {
//移動（実処理）------------------------------------------------------------------------------
        //ダッシュ中なら速度をスケーリングする
        if (!_isDamege && !_animator.GetBool("isAttack")  && !_animator.GetBool("Dead"))
            _rigid.velocity = new Vector3(moveSpeed * _horizontalInput * ((dashLeft || dashRight) ? DASH_SPEED_SCALE : 1.0f), _rigid.velocity.y, _rigid.velocity.z);
    }
}
