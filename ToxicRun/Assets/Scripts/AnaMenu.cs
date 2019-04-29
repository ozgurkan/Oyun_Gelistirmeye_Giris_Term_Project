﻿using UnityEngine;

// Ana menü UI objesinin kullandığı script
public class AnaMenu : MonoBehaviour
{
	// oyun başladığında aktif edilecek scriptler
	public KameraKontrol kameraKontrol;
	public Animation kameraAnimation;
	public Player playerScript;
	public Animation playerAnimation;
	public SkorScript skorScript;
	
	// credits yazısını yazdıran UI objesi
	public UnityEngine.UI.Text creditsText;
	
	// butonlara tıklayınca çıkan ses
	//public AudioClip butonClickSesi;

    void Start()
	{
		// Önceki oyun bitiminde timeScale 0 yapıldıysa diye
		//menüye giriş yapınca timeScale'i tekrar 1 yap
		Time.timeScale = 1f;
	}
	
	// Oyunu Başlat butonuna tıklayınca yapılacaklar
	public void OyunuBaslat()
	{
		// enable edilmesi gereke scriptleri enable et
		kameraKontrol.enabled = true;
		playerScript.enabled = true;
		playerAnimation.enabled = true;
		skorScript.enabled = true;
        
		
		// kameranın loop halinde tekrarladığı animasyona son ver
		//Destroy( kameraAnimation );
		
		// Oyunun başında scene'de yer alan BaslangicZemin objelerini yok et
		DestroyMe.destroy = true;
		
		// butona tıklama sesi çal
		//AudioSource.PlayClipAtPoint( butonClickSesi, Vector3.zero );
    

		
		// MainMenu objesini (ana menüyü) yok et
		Destroy( gameObject );
	}
	
	// Yapımcı butonuna tıklayınca yapılacaklar
	public void Credits()
	{
		// yapımcı butonunun yazısını değiştir
		creditsText.text = "SneakyTeam@";
		
		// 1 saniye sonra credits butonunun yazısını eski haline getir
		// ( CreditsDuzelt() fonksiyonu ve Invoke() fonksiyonu vasıtasıyla )
		CancelInvoke();
		Invoke( "CreditsDuzelt", 1f );

        // butona tıklama sesi çal
        //AudioSource.PlayClipAtPoint(butonClickSesi, Vector3.zero);
    }
	
	// Çıkış butonuna tıklayınca yapılacaklar
	public void Cikis()
	{
        // butona tıklama sesi çal
        //AudioSource.PlayClipAtPoint(butonClickSesi, Vector3.zero);

        // oyunu kapat
        Application.Quit();
	}
	
	void CreditsDuzelt()
	{
		// credits butonunun yazısını eski haline getir
		creditsText.text = "Yapımcı";
	}
}
