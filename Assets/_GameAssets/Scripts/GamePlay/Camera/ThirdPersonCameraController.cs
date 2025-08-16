using UnityEngine;

/// <summary>
/// Bu C# script'i, üçüncü şahıs bir kameranın bakış açısını referans alarak, oyuncunun klavye girdilerine göre 
/// hareket etmesi gereken yönü hesaplar ve karakterin görsel modelini bu yöne doğru yumuşak bir şekilde döndürür.
/// </summary>

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _orientationTransform;
    [SerializeField] private Transform _playerVisualTransform;

    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        Vector3 viewDirection = _playerTransform.position - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);
        _orientationTransform.forward = viewDirection.normalized;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = _orientationTransform.forward * verticalInput + _orientationTransform.right * horizontalInput;

        // _playerVisualTransform.forward: Başlangıç yönü (Karakter şu an nereye bakıyor?).

        // inputDirection.normalized: Hedef yön (Karakterin nereye bakmasını istiyoruz?).

        // Time.deltaTime * _rotationSpeed: Dönüş hızı. Time.deltaTime, bir önceki kareden bu yana geçen süredir. 
        // Bunu hızla çarparak dönüşün bilgisayarın performansından etkilenmeden her zaman aynı hızda ve pürüzsüz olmasını sağlarız.
        if (inputDirection != Vector3.zero)
        {
            _playerVisualTransform.forward = Vector3.Slerp(_playerVisualTransform.forward, inputDirection.normalized, Time.deltaTime * _rotationSpeed);
        }
    }

}
