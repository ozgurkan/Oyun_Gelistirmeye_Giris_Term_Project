﻿using UnityEngine;

// Player'ı kontrol eden script
// Bu script düzgün çalışabilmek için aynı objeye atanmış
// bir InputManager scripti gerektirmekte 
[RequireComponent( typeof( InputManager ) )]
public class Player : MonoBehaviour 
{
	public SonsuzYolScript yolGenerator;
	public SkorScript skor;
	public GameObject gameOverMenu;
	
	// oyun bitince çalan ses dosyası
	public AudioClip gameOverSesi;
	// puan toplayınca çalan ses dosyası
	public AudioClip puanSesi;
	
	// Objenin sahip olduğu ve sık sık eriştiğimiz tüm component'leri
	// birer değişkende tutuyoruz, böylece onlara erişmemiz daha hızlı oluyor
	private Transform tr;
	private Rigidbody r;
	private Animation anim;
	private AudioSource ses;
	private InputManager inputManager;
	
	// Karakterin koşma hızı
	public float maksimumHiz = 10f;
	// Karakterin sağa-sola hareket etme hızı
	public float yatayHiz = 10f;
	
	// Her 10 saniyede bir karakterin koşma hızı (maksimumHiz)
	// 1 artacak (oyun gittikçe hızlanacak/zorlaşacak)
	public float hizArtmaAraligi = 10f;
	private float maksimumHizArtmaZamani;
	
	// Boşluktan aşağı düşerken true olan bir değişken
	private bool ucurum = false;
	// Karakter ölünce true olan bir değişken
	private bool death = false;

    /*Karakterin sağa ve sola ne kadar hareket edebileceği*/
    [HideInInspector]
    public float limitMinDeger = -17f;
    [HideInInspector]
    public float limitMaxDeger = 17f;
	
	// -1 sol
	// 0 hiçbir yön
	// 1 sağ
	// Kavşağa gelince hangi yöne döneceğimizi belirleyen değişken
	private int donusYonu = 0;
	// Kavşağa girip girmediğimizi belirleyen değişken
	private bool donemecteyiz = false;
	
	// Karakterin yüzünü döndüğü yön ileriYon'de, kendisine göre doğu yönü
	// ise sagYon'de depolamakta. Bu değişkenleri kullanarak karakteri o yönlerde hareket ettiriyoruz.
	[HideInInspector]
	public Vector3 ileriYon = Vector3.forward;
	[HideInInspector]
	public Vector3 sagYon = Vector3.right;
	
	void Start()
	{
		r = GetComponent<Rigidbody>();
		tr = transform;
		anim = GetComponentInChildren<Animation>();
		ses = GetComponent<AudioSource>();
		inputManager = GetComponent<InputManager>();
		
		// Player scripti oyunun başında disable edilmiş konumda ve
		// oyunu başlatınca AnaMenu scripti tarafında aktif edilmekte.
		// Bu script aktif olduktan hemen sonra InputManager scriptini aktif
		// ediyor ve InputManager scriptinin Start fonksiyonunda 
		// mouseIlkPozisyon değişkeni ilk değerini alıyor. Yani tam oyunu 
		// başlattığımız anda fare imlecinin bulunduğu konum default konum oluyor
		// ve fare o konumun sağında/solunda ise karakteri ona göre yatay
		// eksende sağa/sola oynatıyoruz
		inputManager.enabled = true;
		
		// koşma hızının artacağı zamanı hesapla
		maksimumHizArtmaZamani = Time.time + hizArtmaAraligi;
	}
	
	void FixedUpdate()
	{
		// Karakter (player) hâlâ hayattaysa:
		if( !ucurum && !death )
		{
			// karakteri ileri yönde hareket ettir
			tr.Translate( Vector3.forward * maksimumHiz * Time.deltaTime );
			// karakteri yatay eksende hareket ettir
			// yatay eksende hareket miktarı için gerekli input'u InputManager scriptinden al
			tr.Translate( Vector3.right * inputManager.YatayInputAl() * yatayHiz * Time.deltaTime );
		}
	}
	
	void Update()
	{
		// Bu fonksiyonda karakterin mevcut pozisyonuna sıklıkla erişeceğimiz için bu pozisyonu bir değişkende tut
		Vector3 konum = tr.position;
			
		// Eğer karakterin dikey eksendeki konumu çok alçaksa (karakter bir boşluktan epeyce aşağıya düşmüşse) oyunu bitir
		if( !death && konum.y < -5f )
		{
			death = true;
			GameOver();
			Time.timeScale = 0f;
		}
		
		// Eğer karakter hâlâ hayattaysa yapılacaklar:
		if( !ucurum && !death )
		{
			// Skoru, karakterin koşma hızıyla doğru orantılı bir şekilde artır
			skor.SkoruGuncelle( Time.deltaTime * maksimumHiz );
			
			// Eğer karakter dikey eksende 1.9f'ten daha alçak bir konumdaysa boşluktan
			// düşmek üzeredir ve bu durumda ucurum değişkeninin değerini true yap.
			// ucurum'un değeri true olduğu zaman karakteri hareket ettirmek
			// mümkün olmuyor ve oyuncu da karakterin düşüşünü üzgün bir ifadeyle
			// izlemek zorunda kalıyor (ta ki karakterin y'si -5'e ulaşıp da oyun bitene kadar)
			if( konum.y < 20f )
			{
				ucurum = true;
				
				// karakter boşluktan düşeceği vakit collider'ını kapatıyoruz.
				GetComponentInChildren<Collider>().enabled = false;
			}
			
			// eğer karakterin koşma hızını artırmanın vakti geldiyse koşma hızını artır ve
			// hızın artırılacağı bir sonraki anı hesapla
			if( Time.time >= maksimumHizArtmaZamani )
			{
				maksimumHizArtmaZamani = Time.time + hizArtmaAraligi;
				maksimumHiz++;
			}
			
			// Karakterin yatay eksendeki hareketini minimum ve maksimum limit değerler aralığıyla sınırla
			if( ileriYon == Vector3.forward || ileriYon == Vector3.back )
				konum.x = Mathf.Clamp( konum.x, limitMinDeger, limitMaxDeger );
			else
				konum.z = Mathf.Clamp( konum.z, limitMinDeger, limitMaxDeger );
			
			tr.position = konum;
			
			// Eğer bir kavşaktaysak ve kavşaktan sağa veya sola dönmek için bir komut (input)
			// vermişsek bu inputu SonsuzYolScript'e (yolGenerator) ilet. Böylece eğer o yönde
			// dönüş yapmak mümkünse SonsuzYolScript infinite yolu o yönde devam ettirecek
			// ve karakteri o yönde döndürecek
			if( donemecteyiz && donusYonu != 0 )
			{
				if( donusYonu == -1 )
				{
					yolGenerator.sol = true;
				}
				else
				{
					yolGenerator.sag = true;
				}
			
				donemecteyiz = false;
				donusYonu = 0;
			}
		}
	}
	
	// Karakteri zıplatmaya yarayan fonksiyon
	public void Zipla()
	{
		// Eğer karakter hâlâ hayattaysa ve bir yerlerden aşağı düşmüyorsa 
		// (dikey eksendeki hızı çok azsa) karakterin zıplama animasyonunu oynat.
		// Burada Blend fonksiyonu kullanıyoruz çünkü karakter zıplarken aynı
		// zamanda koşma animasyonunun da oynamaya devam etmesini istiyoruz
		// ve Blend de tam olarak bunu yapıyor: hali hazırda oynamakta olan animasyonla
		// ismi girilen animasyonu harmanlıyor
		if( !ucurum && !death && Mathf.Abs( r.velocity.y ) < 0.5f )
			anim.Blend( "ZiplamaAnim" );
	}
	
	// Karaktere, kavşağa gelince sola dönmesini söyleyen fonksiyon
	public void SolaDon()
	{
		// eğer karakter hâlâ hayattaysa
		if( !ucurum && !death )
		{
			// sola dönme inputunu ayarla
			donusYonu = -1;
			
			CancelInvoke();
			Invoke( "DonusYonunuResetle", 1f );
		}
	}
	
	// Karaktere, kavşağa gelince sağa dönmesini söyleyen fonksiyon
	public void SagaDon()
	{
		// eğer karakter hâlâ hayattaysa
		if( !ucurum && !death )
		{
			// sağa dönme inputunu ayarla
			donusYonu = 1;
			
			// 1 saniye sonra DonusYonunuResetle fonksiyonunu çağır
			// Bu fonksiyon donusYonu'nu geri 0 yapar (resetler).
			CancelInvoke();
			Invoke( "DonusYonunuResetle", 1f );
		}
	}
	
	// Kavşaktan sağa veya sola dönme inputunu resetleyen fonksiyon
	void DonusYonunuResetle()
	{
		donusYonu = 0;
	}
	
	// Oyun bitince yapılacak şeyler
	void GameOver()
	{
		death = true;
		
		// Yüksekskor yapmışsak bu skoru diske kaydet
		skor.YuksekSkorKaydet();
		
		// Game Over menüsünü aktif et 
		// (bu menü oyunun başında kapalı (inaktif) vaziyette)
		gameOverMenu.SetActive( true );
		
		// Oyun bitme sesini çal
		ses.PlayOneShot( gameOverSesi );
	}
	
	// Player, collider'ının Is Trigger'ı işaretli başka bir objeyle
	// "temas edince" bu fonksiyon çağrılır ve temas edilen obje
	// c isimli parametrede depolanır
	void OnTriggerEnter( Collider c )
	{
		if( c.tag == "Donemec" )
		{
			// Eğer kavşak objesinin temas alanıyla temas etmişsek:
			// kavşağa girdiğimizi belirleyen değişkeni true yap
			donemecteyiz = true;
		}
		else if( c.tag == "PuanObjesi" )
		{
			// Eğer bir puan objesine dokunmuşsak:
			// skoru arttır
			skor.SkoruGuncelle( 100 );
			
			// skor objesini inaktif yap ve havuza (pool) geri yolla
			// (yani skor objesini Destroy yapmıyoruz!)
			c.gameObject.SetActive( false );
			yolGenerator.PuanObjesiniHavuzaYolla( c.transform );
			
			// puan toplama sesi çal
			ses.PlayOneShot( puanSesi );
		}
	}
	
	// Player, collider'ının Is Trigger'ı işaretli başka bir objeyle
	// temasını "kesince" bu fonksiyon çağrılır ve teması kesilen obje
	// c isimli parametrede depolanır
	void OnTriggerExit( Collider c )
	{
		// eğer kavşaktan çıkmışsak kavşak değişkenini false yap
		if( c.tag == "Donemec" )
			donemecteyiz = false;
	}
	
	// Player, collider'ının Is Trigger'ı işaretli "olmayan" başka bir objeyle
	// "temas edince" bu fonksiyon çağrılır ve temas edilen obje
	// c isimli parametrede depolanır
	void OnCollisionEnter( Collision c )
	{
		// eğer temas edilen objenin tag'ı "OlumculEngel" ise
		// (bu tag'ı çarpılabilir duvar objelerine verdim):
		if( c.gameObject.tag == "OlumculEngel" && !death )
		{
			// Rigidbody'i kapat
			r.isKinematic = true;
			// Karakterin ölme animasyonunu oynat
			anim.Play( "OlmeAnimasyonu" );
			// Karakter ölünce yapılacak işlemleri gerçekleştir
			GameOver();
		}
	}
}
