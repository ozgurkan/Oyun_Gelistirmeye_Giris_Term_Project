using UnityEngine;

// Karakteri (Player) hareket ettirirken kullandığımız
// inputları düzenli bir şekilde depolayan, kontrol eden script

public class InputManager : MonoBehaviour 
{
	// Player scriptini depolayan değişken
	private Player player;	
	private float mouseIlkPozisyon;

	void Start()
	{
		player = GetComponent<Player>();		
		// Karakteri hareket ettirirken mouse'nin son konumundan ilk konumunu çıkarıyoruz
		// Alttaki değişken mousenin ilk konumunu depolamaya yarıyor
		mouseIlkPozisyon = Input.mousePosition.x;
	}
	
	void Update() 
	{			
		// Eğer space tuşuna veya farenin sol tuşuna basılırsa player'ı zıplat
		if( Input.GetKeyDown( KeyCode.Space ) || Input.GetMouseButtonDown( 0 ) )
		{
			player.Zipla();
		}		
		// Eğer A tuşuna basılırsa karakteri sol yol ayrımına saptır
		if( Input.GetKeyDown( KeyCode.A ) )
		{
			player.SolaDon();
		}		
		// Eğer D tuşuna basılırsa karakteri sağ yol ayrımına saptır
		if( Input.GetKeyDown( KeyCode.D ) )
		{
			player.SagaDon();
		}
		
	}
	
	// Karakteri yatay eksende hareket ettirirken bu hareketin miktarını 
	// belirlemeye yarayan input'u alırken kullandığımız fonksiyon
	public float YatayInputAl()
	{	
	// mouse'nin son konumu ile ilk konumu arasındaki farkı bulup 50'ye böl ve bu değerin -1 ile 1 arasında olmasını sağla
		return Mathf.Clamp( ( Input.mousePosition.x - mouseIlkPozisyon ) / 200f, -1f, 1f );

	}
}
