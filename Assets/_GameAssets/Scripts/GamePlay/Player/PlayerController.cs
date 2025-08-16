using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private float _playerSpeed;
    [SerializeField] private KeyCode _movementKey;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _canJump;
    [SerializeField] private float _jumpCooldown;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _movementMultiplier;
    [SerializeField] private float _slideDrag;


    [Header("Ground Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _playerHeight;
    [SerializeField] private float _groundDrag;

    private Rigidbody _playerRigidBody;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;
    private float _raycastDistance = 0.2f;
    private bool _isSliding;

    private void Awake()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
        _playerRigidBody.freezeRotation = true;
    }
    private void Update()
    {
        SetInputs();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    void FixedUpdate()
    {
        SetPlayerMovement();
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
        }
        else if (Input.GetKeyUp(_movementKey))
        {
            _isSliding = false;
        }
        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        // _verticalInput: İleri (+1) veya geri (-1) gitmek için klavyeden gelen değer.
        //_horizontalInput: Sağa(+1) veya sola(-1) gitmek için klavyeden gelen değer.
        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;
        if (_isSliding)
        {
            _playerRigidBody.AddForce(_movementDirection.normalized * _playerSpeed * _movementMultiplier, ForceMode.Force);

        }
        else
        {
            _playerRigidBody.AddForce(_movementDirection.normalized * _playerSpeed, ForceMode.Force);
        }
    }
    private void SetPlayerDrag()
    {
        if (_isSliding)
        {
            _playerRigidBody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidBody.linearDamping = _groundDrag;
        }
    }
    private void LimitPlayerSpeed()
    {
        // _playerRigidBody.linearVelocity nesnesi, karakterin anlık 3 boyutlu hızını içerir.
        Vector3 currentVelocity = _playerRigidBody.linearVelocity;
        Vector3 flatVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        if (flatVelocity.magnitude > _playerSpeed)
        {
            // flatVelocity.normalized, vektörün yönünü koruyarak uzunluğunu 1'e eşitler. Buna birim vektör denir.
            // Bu birim vektör, _playerSpeed ile çarpıldığında, hız vektörünün yönü aynı kalır ancak büyüklüğü tam olarak 
            // _playerSpeed değerine eşitlenir. Yani, karakterin hızı limit değerine düşürülmüş olur.
            Vector3 limitedVelocity = flatVelocity.normalized * _playerSpeed;
            _playerRigidBody.linearVelocity = new Vector3(limitedVelocity.x, currentVelocity.y, limitedVelocity.z);
        }
    }
    private void SetPlayerJumping()
    {
        // Bu satır, karakterin mevcut dikey hızını sıfırlıyor. Yani, karakter düşerken veya bir yamaçtan aşağı kayarken zıplama tuşuna basıldığında,
        // önceki dikey hızı (y eksenindeki hızını) tamamen kesiyor ve ardından zıplama kuvvetini uyguluyor.
        _playerRigidBody.linearVelocity = new Vector3(_playerRigidBody.linearVelocity.x, 0f, _playerRigidBody.linearVelocity.z);
        _playerRigidBody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJumping()
    {
        _canJump = true;
    }
    private bool IsGrounded()
    {
        // Işının nereye gideceğini gösteren görsel bir çizgi çiz.
        // Debug.DrawRay(transform.position, Vector3.down * (_playerHeight * 0.5f + 2f), Color.red);
        /*
            transform.position: Işının nereden başlayacağını belirtir. Yani, karakterin tam orta noktasından başlıyor.

            Vector3.down: Işının hangi yöne gideceğini belirtir. Vector3.down, Y ekseninde aşağı doğru olan yönü temsil eder.
            _playerHeight * 0.5f + 2f: Işının ne kadar uzağa gideceğini (mesafesini) belirler. Buradaki mantık şöyle:
                _playerHeight * 0.5f: Karakterin orta noktasından ayağına kadar olan mesafe.
                + 2f: Bu mesafeye 2 birim daha ekleyerek, karakterin havada zıplamış gibi durumlarda da yere temasını kontrol etmesini sağlıyor.
                Bu hesaplama, karakterin ayaklarının hemen altındaki zemini kontrol etmek için mantıklı bir yaklaşımdır.

            _groundLayer: Işının sadece belirli bir katmandaki (layer) objelerle etkileşime girmesini sağlar. Bu, karakterin havada uçan bir düşman veya 
            bir engel yerine sadece zemin olarak tanımlanan objelerle etkileşim kurmasını garanti altına alır. Bu, çok iyi bir tasarım kararıdır.
        */
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + _raycastDistance, _groundLayer);
    }
}
