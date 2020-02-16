# Nedir ?

Moneymo API'larının authentication işlemlerini gerçekleştiren Rest API servisidir. Veritabanı işlemleri için Entity Framework Core kullanılmıştır.

# Nasıl Çalıştırılır ?

Uygulamanın istenilen portta çalışması için `/src/Moneymo.AuthenticationService.API` klasörü içinde aşağıdaki ayarı içeren bir `hostsettings.json` dosyasının bulunması gerekmektedir. Bu dosya deploy sırasında yaşanabilecek karışıklıkları önlemek amacıyla repodan kaldırılmış ve `.gitignore` dosyasına eklenmiştir.

```
{
  "urls" : "http://*:8893"
}
```

Uygulama API'lar için token oluşturma ve bu tokenların güncelliğini kontrol etme olarak iki temel endpoint içermektedir. Bunlar dışında yeni API kullanıcısı oluşturmak için bir endpoint daha içerir. Yeni bir API kullanıcısı SSH ile sunucuya bağlanıldığında aşağıdaki gibi bir post çağrısı ile oluşturulabilmektedir.

```bash
curl -X POST "http://127.0.0.1:8893/authapi/users" -H "accept: application/json" -H "Content-Type: application/json-patch+json" -d '{"username": "new_user", "password": "GbxmCt123tqu", "fullname":"New User"}'
```

Ayrıca veritabanı bağlantısı için geliştirme ortamında bulunan `secrets.json` dosyasında `ConnectionString` adındaki ayar eklenmiş olması gerekmektedir. Bu ayar `appsettings.Development.json` dosyasında da tutulabilir fakat tavsiye edilen yöntem veritabanı bağlantı bilgilerinin hiçbir şekilde remote repository'de tutulmaması yönünde olduğu için `secrets.json` dosyası kullanılmalıdır.
