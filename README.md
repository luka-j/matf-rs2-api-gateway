# [MATF RS2] API Gateway

Implementacija [API Gateway](https://microservices.io/patterns/apigateway.html) sistema kao 4 ASP.NET mikroservisa + React upravljački panel.
Projekat za kurs [Razvoj softvera](https://matfrs2.github.io/RS2/) 2 @ [MATF](http://www.matf.bg.ac.rs).

## Slučaj(evi) upotrebe

API Gateway sistemi su najkorisniji u razvijenim mikroservisnim arhitekturama gde jednostavno proksiranje zahteva nije dovoljno, a pravljenje
odvojenih mikroservisa svaki put kada obrnuti proksi ne zadovoljava potrebe je overkill. Recimo, ako postoji interni API koji podržava
veliki skup funkcionalnosti, a želimo da određenim grupama izložimo samo podskupove ovog APIja. Ovaj sistem takođe može pomoći pri održavanju
stabilnosti javnog APIja, dopuštajući da se interni APIji menjaju i unapređuju, kao i olakšati migracije. Takođe, kako je ovo ulazna ili skoro
ulazna tačka sistema, ovde je idealno mesto da se zahtevi validiraju, srede, loguju i da se prate metrike celog sistema.

Ukratko, cilj API Gatewaya je da eliminiše mikroservise čija je jedina svrha "presipanje" i prepakivanje zahteva i da standardizuje i
centralizuje taj posao.

## Pregled sistema
Sistem možemo posmatrati iz dve perspektive: iz ugla zahteva koji dolazi "spolja" (sa javne mreže) i iz ugla upravljača koji konfiguriše sistem.

### Iz ugla zahteva

Zahtev prvo dolazi na `API` mikroservis, koji na osnovu putanje pronalazi odgovarajuću specifikaciju APIja (u terminima obrnutih proksija,
`frontend`a) i validira ga. Ovaj proces podrazumeva parsiranje zaglavlja, parametara i tela (ako postoje), upoređivanje sa specifikacijom,
odbacivanje viška parametara (koji ne postoje uspecifikaciji) i, u slučaju da fali neki od obaveznih parametara, odbijanje zahteva. Isparsiran
zahtev se prosleđuje `Request Processor` (RP) mikroservisu, koji za konkretan API i endpoint pronalazi skup koraka koji treba da izvrši.
Postoji 13 tipova koraka koji se mogu podeliti na manipulaciju podacima (kreiranje i menjanje isparsiranog zahteva), kontrolu toka (if,
foreach, break, return) i interakciju sa "spoljnim svetom" (log, snimanje ili čitanje iz izvora podataka, izvršavanje http zahteva). Ovako
obrađen zahtev se vraća `API` mikroservisu kao `odgovor` koji obavezno sadrži status i opciono zaglavlja i telo, koji pronalazi odgovor u
specifikaciji, izvršava validaciju odgovora (analogno validaciji zahteva) i ako ona prođe, serijalizuje odgovor u odgovarajući HTTP odgovor
koji vraća pozivaocu.

Za interakciju sa izvorima podataka `RP` mikroservis se oslanja na `Common Complex Operations` (CCO) <sup>inicijalno ambicioznije zamišljen</sup>
koji kao konfiguraciju čuva sve podatke neophodne za uspostavljanje konekcije ka Postgres bazama, Redis keševima i RabbitMQ redovima. Kao deo
koraka navodi se samo naziv odgovarajućeg izvora i podaci kojima želimo manipulisati, a `CCO` pronalazi odgovarajuću konfiguraciju i obavlja
komunikaciju sa izvorom podataka. Broj različitih izvora koji sistem može koristiti je (u teoriji) neograničen.

Za pravljenje HTTP zahteva, `RP` zove odgovarajući endpoint `API` mikroservisa. Da bi zahtev uopšte mogao biti napravljen, `API` mikroservis mora
imati specifikaciju servisa koji želimo pozvati (`backend`a). On zahtev i dobijeni odgovor validira i (de)serijalizuje i vraća `RP` mikroservisu.
Ova uloga `API` mikroservisa je konceptualno drugačija od uloge pri primanju zahteva sa javne mreže, ali je logika potpuno analogna, te ima smisla
da se nalazi u istom mikroservisu.

### Iz ugla upravljača
Upravljački panel predstavlja frontend za konfiguraciju celog sistema. Sastoji se od frontenda i `Configurator` (CONF) mikroservisa koji čuva
konfiguracije i zadužen je da ih dostavlja svim mikroservisima.

Da bi se pristupilo upravljačkom panelu, potrebno je ulogovati se pomoću administratorskog naloga. Kao identity provider koristimo
[Zitadel](https://zitadel.com/) instancu i sve naloge koji imaju grant na odgovarajućem klijentu smatramo administratorskim (integracija je
standardni OAuth2). Na ovom panelu se mogu videti i izmeniti frontend konfiguracije, odgovarajući `RP` middleware-i, `backend` konfiguracije,
kao i parametri za pristup izvorima podataka.

API frontend i backend konfiguracije su u standardnom OAS3 formatu i prikazuju se uz odgovarajuću vizualizaciju (slično <editor.swagger.io>).
Middleware-i su u custom YAML formatu koji odgovara `ApiGatewayRequestProcessor.Configs.ApiConfig` klasi. Izvori podataka se postavljaju i menjaju
popunjavanjem polja na frontendu (kasnije se serijalizuju u JSON).

Frontend za upravljački panel komunicira isključivo sa `CONF` projektom, koji sve izmene pohranjuje u fajlove i čuva na privatnom GitHub
repozitorijumu, kako bi uvek postojao trag o svim napravljenim izmenama. On je takođe zadužen za dohvatanje trenutno aktuelnih konfiguracija
na mikroservisima i njihovo menjanje. Ovo je posebno značajan problem u distribuiranim sistemima i `CONF` mikroservis je napravljen tako da
podržava integraciju sa Kubernetesom i proizvoljan broj instanci za svaki od mikroservisa. Svaka konfiguracija ima trenutak od kojeg počinje da
važi, kako bi se obezbedilo da sve instance pređu na novu konfiguraciju u istom trenutku u slučaju izmena (pretpostavljamo sinhronizovanost satova).

## Upotreba

Pristup upravljačkom panelu je moguć na adresi <https://app-rs2.luka-j.rocks> pomoću datih kredencijala. Izloženi APIji su dostupni na
<https://api-rs2.luka-j.rocks/>.

Metrike se mogu videti na Grafana instanci <https://grafana-rs2.luka-j.rocks>, RabbitMQ administratorski panel je na <https://rabbitmq-rs2.luka-j.rocks/>,
dok je ArgoCD (za pregled Kubernetes klastera i deploymenta) dostupan na <https://argo-rs2.luka-j.rocks>. Za pristup svakom od ovih alata
potreban je poseban set kredencijala.

## Infrastruktura i objavljivanje

Svi mikroservisi, monitoring alati, ArgoCD i RabbitMQ su hostovani na Oracle-ovoj cloud infrastrukturi u managed Kubernetes klasteru.
Klaster se sastoji od dve arm64 mašine sa po 5GB RAMa i po 50GB diska.
PostgreSQL instanca korišćena kao demo je managed instanca na Aiven-u. Redis instanca korišćena kao demo je managed instanca na Redis Cloud-u.

Kubernetes klaster i odgovarajuće mrežne postavke se mogu rekreirati pomoću Terraform skripti koje se nalaze u `infra/0-terraform` direktorijumu.
Resursi za postavljanje osnovnih alata u Kubernetesu su dostupni u `infra/1-kubernetes-setup-resources`. Ovaj deo procesa nije automatizovan,
ali se svodi na apply-ovanje resursa i instalaciju Helm chart-ova. Nakon što se instalira ArgoCD i primeni Argo app-of-apps, Argo će podići
mikroservise i sve ostale alate. U klasteru se koristi nginx kao ingress controller i load balancer, a cert-manager obezbeđuje HTTPS sertifikate
za sve poddomene koji se koriste (oni se automatski provision-uju i osvežavaju, što je standardna praksa).

Push na master granu u neki od direktorijuma mikroservisa automatski pokreće pipeline koji izgrađuje mikroservis i startuje objavljivanje na Argu
nakon toga, što omogućava zero click deployment u klaster.

Obratiti pažnju da su mašine u klasteru arm64: ako pokušate da pokrenete bilo koji od image-a na nekoj drugoj arhitekturi, to vam neće poći za rukom.
Dockerfile-ovi koji izgrađuju ove image-e su pisani tako da mogu da se izvrše na amd64 mašinama (pošto GitHub builderi koriste tu arhitekturu),
ali je rezultujući image kompatibilan samo sa arm64.

Detaljan opis CI/CD procesa je dostupan u `CICD.md` fajlu.

## Članovi tima
- Pavle Cvejović 1068/2022
- Viktor Novaković 1063/2022
- Luka Jovičić 1067/2022