# Algorytm genetyczny dla problemu komiwojażera

## Opis
Projekt implementuje algorytm genetyczny do rozwiązania problemu komiwojażera (TSP). Algorytm korzysta z reprezentacji permutacyjnej miast oraz operatorów krzyżowania i mutacji.

## Dokumentacja
- [Dokumentacja formatu TSPLIB](https://oeclass.aua.gr/eclass/modules/document/file.php/310/3.%20%CE%91%CF%81%CF%87%CE%B5%CE%AF%CE%B1%20VRP%20-%20format.pdf)

## Implementacja
### Zrealizowane
- **Wczytywanie danych w formacie TSPLIB**: Import pliku TSPLIB, wczytywanie danych o miastach (współrzędne), tworzenie macierzy odległości.
- **Algorytm genetyczny z reprezentacją permutacyjną**: Algorytm genetyczny operujący na permutacji miast, uwzględniający populację, selekcję turniejową, krzyżowanie i mutację.
- **Operator mutacji (swap)**: Prosta mutacja polegająca na zamianie dwóch losowych miast w chromosomie.
- **Operator krzyżowania PMX**: Krzyżowanie PMX.
- **Implementacja drugiego operatora krzyżowania OX**: Krzyżowanie OX, operator wybierany losowo.
- **Algorytm memetyczny (lokalna optymalizacja nowych rozwiązań)**: Po każdym krzyżowaniu i mutacji, zastosowanie lokalnej optymalizacji (np. 2-opt) do poprawienia rozwiązania. Opcjonalnie, zastąpienie 2-opt bardziej zaawansowaną heurystyką (3-opt lub Lin-Kernighan).
- **Generator grafów losowych i porównanie z heurystyką 2-opt**: Stworzenie generatora losowej instancji TSP (losowe położenie miast) oraz zaimplementowanie heurystyki 2-opt.

## Jak uruchomić
Aby uruchomić projekt po pobraniu repozytorium, użyj następujących komend:

```sh
dotnet run -- --input-file <ścieżka_do_pliku_TSPLIB> [opcje]
```

### Parametry
- `--input-file <path>`: Ścieżka do pliku TSPLIB z danymi. Jeśli nie zostanie podana, generowany jest losowy graf.
- `--cities <int>`: Liczba miast dla generatora losowego grafu (domyślnie: 50).
- `--range <int>`: Zakres współrzędnych dla generatora losowego grafu (domyślnie: 100).
- `--solver <string>`: Metoda rozwiązania: 'GA' dla algorytmu genetycznego, '2OPT' dla heurystyki 2-opt (domyślnie: 'GA').
- `--population <int>`: Wielkość populacji dla algorytmu genetycznego (domyślnie: 100).
- `--generations <int>`: Liczba generacji dla algorytmu genetycznego (domyślnie: 1000).
- `--mutation-rate <double>`: Wskaźnik mutacji dla algorytmu genetycznego (domyślnie: 0.05).
- `--crossover-rate <double>`: Wskaźnik krzyżowania dla algorytmu genetycznego (domyślnie: 0.9).
- `--crossover-method <string>`: Metoda krzyżowania: 'PMX' (Partially Mapped Crossover) lub 'OX' (Order Crossover) (domyślnie: 'PMX').
- `--heuristic-method <string>`: Metoda heurystyczna dla algorytmu memetycznego: '2OPT' lub '3OPT' (domyślnie: 'LK').
- `--debug <bool>`: Włączenie lub wyłączenie debugowania w funkcji GeneticTSPSolver.Solve() (domyślnie: true).
- `--compare <bool>`: Porównanie wyników algorytmu genetycznego i heurystyki 2-opt (domyślnie: false).
- `--runs <int>`: Liczba uruchomień do uśredniania wyników (domyślnie: 0).

### Przykłady użycia
```sh
dotnet run -- --input-file data.tsp --solver GA --population 200 --generations 500
dotnet run -- --cities 100 --range 200 --solver 2OPT
dotnet run -- --input-file data.tsp --compare true --runs 5
```
