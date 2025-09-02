# Flappy Bird 

Jednostavna desktop igrica napravljena u **C# / .NET (WPF)**.  
Prije početka **obavezno je odabrati pticu**, zatim igra kreće na SPACE.  
Igra ima **dnevni/noćni ciklus** (svakih **30 sekundi**), pamti **Top 3 rezultata** i ima opciju **gašenja zvuka**.

---

## Kako igrati

- **SPACE** – skok / poleti
- **Enter / “Započni igru”** – start
- **P** – pauza (ako je implementirano u tvojoj verziji)
- **Esc** – izlaz iz igre
- **Klik na “Zvuk: On/Off”** – uključivanje/isključivanje zvuka
- **Klik na “Odaberi pticu”** – izbor lika (obavezno prije starta)

> Ako ptica udari u cijev ili tlo → **Game Over**.  
> Nakon Game Over-a možeš ponovo startati igru tipkom **SPACE** ili dugmetom **“Nova igra”** (ako postoji).

---

## Funkcionalnosti

- **Obavezno biranje ptice** prije početka (poseban prozor / dijalog)
- **Dan / Noć**: tema scene se automatski mijenja svaka **30 s**
- **Top lista (Top 3)**: čuva se lokalno (npr. u JSON/Settings); prikaz u meniju *Top rezultati*
- **Zvuk**: efekti skoka/sudara; **toggle On/Off**
- **Score**: +1 po uspješno prošloj prepreci
- **Game Over ekran**: trenutni rezultat + najbolji rezultat

---

##  Pokretanje

### Preduslovi
- **.NET SDK 8** (ili verzija projekta)
- **Visual Studio 2022** (Desktop development with .NET)

### Start
```bash
git clone https://github.com/<tvoj-username>/FlappyBird-WPF.git
cd FlappyBird-WPF
# Otvoriti .sln u Visual Studio i pokrenuti (F5)
