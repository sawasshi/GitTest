using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ぎゃばのリーダーうんちってCLに書いて出したらしいでwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww

// 勝手にマージするなよ

// おい

public class Player : MonoBehaviour
{
    //=======================================================
    // 基本的に何も考えず作ったので改良の余地大幅にアリ
    //=======================================================

    [SerializeField, Header("プレイヤー上昇スピード")]
    private float PlayerUpSpeed = 0.0f;

    [SerializeField, Header("プレイヤー横移動スピード")]
    private float PlayerSpeed = 0.0f;

    [SerializeField, Header("重力の強さ")]
    private float PlayerGravity = 0.0f;

    // 落ちたフラグ
    private bool PlayerFallFlg = false;

    // Rigidbodyを取得
    Rigidbody rb;

    // 画面振動用
    GameObject maincamera;
    public CameraShake shake;

    // 移動量補間
    float MoveAmount = 0;
    float MoveLeft = 0;
    float MoveUp = 0;
    float MoveDown = 0;

    // AIMフラグ
    bool TempMoveMAFlg = false;

    // ストップフラグ
    bool StopFlg = false;

    // 角度
    float Angle = 90;

    // 方向構造体
    public struct DirectionFlg
    {
        public bool right;
        public bool left;
        public bool up;
        public bool down;
        public DirectionFlg(bool _right, bool _left, bool _up,bool _down)
        {
            this.right = _right;
            this.left = _left;
            this.up = _up;
            this.down = _down;
        }
    }
    

    // 方向フラグ
    DirectionFlg directionFlg = new DirectionFlg(false,false,false,false);

    // Start is called before the first frame update
    void Start()
    {
        // 画面振動用にカメラとスクリプトを取得
        maincamera = GameObject.Find("Main Camera");
        shake = maincamera.GetComponent<CameraShake>();
        // Rigidbodyを取得
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerFallFlg)                 // 落ちる時
        {
            // 重力停止
            rb.velocity = Vector3.zero;

            // 上昇
            PlayerUp();
        }
        else if (!StopFlg)                 // 落ちてない時
        {
            // 下降
            PlayerDown();
        }

        // 左キー押したとき(Trigger)
        if (Input.GetKeyDown("left"))
        {
            TempMoveMAFlg = false;
            directionFlg.left = true;
        }

        // 右キー押したとき(Trigger)
        if (Input.GetKeyDown("right") && StopFlg)
        {
            TempMoveMAFlg = false;
            directionFlg.right = true;
        }

        // 上キー押したとき(Trigger)
        if (Input.GetKeyDown("up"))
        {
            this.transform.Translate(0.0f, 0.0f, 2.0f, Space.World);
        }

        // 下キー押したとき(Trigger)
        if (Input.GetKeyDown("down"))
        {
            this.transform.Translate(0.0f, 0.0f, -2.0f, Space.World);
        }

        // 右移動
        if (directionFlg.right)
        {
            if (PlayerMove(2.0f, true,directionFlg)) { }
            else
            {
                directionFlg.right = false;
                StopFlg = false;
            }
        }

        // 左移動
        if (directionFlg.left)
        {
            if (PlayerMove(2.0f, true, directionFlg)) { }
            else
            {
                directionFlg.left = false;
                StopFlg = false;
            }
        }

        // 手前移動
        if (directionFlg.down)
        {
            if (PlayerMove(2.0f, true, directionFlg)) { }
            else
            {
                directionFlg.down = false;
                StopFlg = false;
            }
        }

        // 奥移動
        if (directionFlg.up)
        {
            if (PlayerMove(2.0f, true, directionFlg)) { }
            else
            {
                directionFlg.down = false;
                StopFlg = false;
            }
        }
    }

    // collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Surface")) // パジャマ？に当たったら
        {
            PlayerFallFlg = true;
            shake.Shake(0.25f, 0.1f);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Ceiling")) // 天井に当たったら
        {
            PlayerFallFlg = false;
            StopFlg = true;
        }
        if (other.gameObject.CompareTag("Arm"))     // 腕（地面）に当たったら
        {
            shake.Shake(0.25f, 0.1f);
            PlayerFallFlg = true;
        }
        if (other.gameObject.CompareTag("CrackedSurface"))  // ヒビ割れパジャマに当たったら
        {
            shake.Shake(0.25f, 0.1f);
            /////////////////////////////
            // 連鎖して壊していく処理 ///
            /////////////////////////////
            PlayerFallFlg = true;
        }
    }

    // サブ関数
    private bool PlayerUp()
    {
        // 上昇
        this.transform.Translate(0.0f, PlayerUpSpeed, 0.0f);
        return true;
    }

    private void PlayerDown()
    {
        // 下降（重力Ver）
        rb.AddForce(0.0f, -PlayerGravity, 0.0f);

        // 下降（等速直線運動）
        //this.transform.Translate(0.0f, -player_speed, 0.0f);
    }

    private bool PlayerRotate(float _angle)
    {
        // Transformを取得
        Transform myTransform = this.transform;
        if (Angle > 0)
        {
            // Z軸回転
            myTransform.Rotate(0.0f, 0.0f, -5.0f);
            Angle -= 5.0f;
            return true;
        }
        return false;
    }

    // to do あとで変えとけ汚すぎ
    private bool PlayerMove(float _amount, bool _flg,DirectionFlg _dir)
    {
        if (!TempMoveMAFlg)    // 呼ばれた時に一度だけ移動量を指定したい！
        {
            MoveAmount = _amount;
            TempMoveMAFlg = _flg;
        }
        else
        {    // 呼ばれる間繰り返す移動処理
            if (_dir.right) this.transform.Translate(PlayerSpeed, 0.0f, 0.0f, Space.World);
            if (_dir.left) this.transform.Translate(-PlayerSpeed, 0.0f, 0.0f, Space.World);
            if (_dir.up) this.transform.Translate(0.0f, 0.0f, PlayerSpeed, Space.World);
            if (_dir.down) this.transform.Translate(0.0f, -PlayerSpeed, 0.0f, Space.World);
            MoveAmount -= PlayerSpeed;
            if (MoveAmount <= 0)
            {
                TempMoveMAFlg = false;
                return false;
            }
        }
        return true;
    }
}

//=======================================================
// End of File
//=======================================================