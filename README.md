Algorytm genetyczny dla problemu komiwojażera

[Dokumentacja formatu TSPLIB](https://oeclass.aua.gr/eclass/modules/document/file.php/310/3.%20%CE%91%CF%81%CF%87%CE%B5%CE%AF%CE%B1%20VRP%20-%20format.pdf)

![Wzrór na odległość](https://github.com/user-attachments/assets/95f6ec69-760b-4dba-83d0-68eea74657fd)

### Zrealizowane

- **Wczytywanie danych w formacie TSPLIB**:  
  Zaimportowano plik TSPLIB, wczytywane są dane o miastach (współrzędne), tworzona jest macierz odległości.

- **Algorytm genetyczny z reprezentacją permutacyjną**:  
  Zaimplementowany algorytm genetyczny operujący na permutacji miast, uwzględniający populację, selekcję turniejową, krzyżowanie i mutację.

- **Operator mutacji (swap)**:  
  Dodano prostą mutację polegającą na zamianie dwóch losowych miast w chromosomie.

- **Operator krzyżowania PMX**:  
  Krzyżowanie PMX.

- **Implementacja drugiego operatora krzyżowania - OX**:
  Krzyżowanie OX, operator wybierany losowo 

### Do zrobienia

- **Dodatkowe operatory krzyżowania**:

  - Implementacja trzeciego operatora krzyżowania (np. CX), aby spełnić wymagania i zdobyć dodatkowe punkty.

- **Generator grafów losowych i porównanie z heurystyką 2-opt**:

  - Stworzyć generator losowej instancji TSP (losowe położenie miast).
  - Zaimplementować heurystykę 2-opt.
  - Porównać wyniki GA z wynikami 2-opt na wygenerowanych instancjach.

- **Algorytm memetyczny (lokalna optymalizacja nowych rozwiązań)**:
  - Po każdym krzyżowaniu i mutacji, zastosować lokalną optymalizację (np. 2-opt) do poprawienia rozwiązania.
  - Opcjonalnie (dodatkowe punkty): zastąpić 2-opt bardziej zaawansowaną heurystyką (3-opt lub Lin-Kernighan) w algorytmie memetycznym.

### Bonusowe

- **3-opt (lokalna optymalizacja)**:  
  Wprowadzić 3-opt jako ulepszenie w algorytmie memetycznym (+10 pkt).

- **Lin-Kernighan (LK)**:  
  Wprowadzić heurystykę Lin-Kernighan w algorytmie memetycznym (+15 pkt).
