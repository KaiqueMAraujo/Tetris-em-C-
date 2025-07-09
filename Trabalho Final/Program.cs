using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace TetrisGame
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] tabuleiro = new int[20, 10];
            string[] tipos = { "T", "L", "I" };
            Random rand = new Random();

            Console.Write("Digite seu nome: ");
            string nome = Console.ReadLine();
            Jogador jogador = new Jogador(nome);

            bool jogoAtivo = true;

            while (jogoAtivo)
            {
                string tipo = tipos[rand.Next(tipos.Length)];
                Tetrominos peca = new Tetrominos(tipo);
                int linhaAtual = 0;
                int colunaAtual = 3;

                if (!PodeDescer(peca.Matriz, linhaAtual, colunaAtual, tabuleiro))
                {
                    Console.WriteLine("\n  Jogo Encerrado! O tabuleiro está cheio.");
                    jogoAtivo = false;
                    break;
                }

                while (PodeDescer(peca.Matriz, linhaAtual, colunaAtual, tabuleiro))
                {
                    Console.Clear();
                    DesenharTabuleiroComPeca(tabuleiro, peca.Matriz, linhaAtual, colunaAtual);

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo tecla = Console.ReadKey(true);

                        switch (tecla.Key)
                        {
                            case ConsoleKey.A:
                                if (colunaAtual > 0)
                                    colunaAtual--;
                                break;

                            case ConsoleKey.D:
                                if (colunaAtual < 10 - 3)
                                    colunaAtual++;
                                break;

                            case ConsoleKey.S:
                                if (PodeDescer(peca.Matriz, linhaAtual, colunaAtual, tabuleiro))
                                    linhaAtual++;
                                break;

                            case ConsoleKey.W:
                                peca.RotacionarHorario();
                                break;
                        }
                    }

                    Thread.Sleep(200);
                }

                FixarPeca(peca.Matriz, linhaAtual, colunaAtual, tabuleiro);

                jogador.AdicionarPontos(tipo == "I" ? 3 : tipo == "L" ? 4 : 5);

                int linhasRemovidas = RemoverLinhas(tabuleiro);
                if (linhasRemovidas > 0)
                {
                    jogador.AdicionarPontos(300 * linhasRemovidas);
                    if (linhasRemovidas > 1) jogador.AdicionarPontos(100);
                }
            }

            Console.Clear();
            Console.WriteLine("TABULEIRO FINAL:");
            ImprimirTabuleiro(tabuleiro);
            Console.WriteLine($"\n{jogador}");

            jogador.SalvarPontuacao("scores.txt");
            Console.WriteLine("Pontuação salva em scores.txt");
        }

        static void ImprimirTabuleiro(int[,] tab)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(tab[i, j] == 0 ? "⬜" : "◼");
                }
                Console.WriteLine();
            }
        }

        static void DesenharTabuleiroComPeca(int[,] tab, int[,] peca, int linha, int coluna)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    bool desenhouPeca = false;

                    for (int pi = 0; pi < 3; pi++)
                    {
                        for (int pj = 0; pj < 3; pj++)
                        {
                            if (peca[pi, pj] == 1 &&
                                i == linha + pi &&
                                j == coluna + pj)
                            {
                                Console.Write("⬛ ");
                                desenhouPeca = true;
                            }
                        }
                    }

                    if (!desenhouPeca)
                        Console.Write(tab[i, j] == 0 ? "⬜ " : "◼ ");
                }
                Console.WriteLine();
            }
        }

        static bool PodeDescer(int[,] peca, int linhaAtual, int colunaAtual, int[,] tabuleiro)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (peca[i, j] == 1)
                    {
                        int linha = linhaAtual + i + 1;
                        int coluna = colunaAtual + j;

                        if (linha >= 20 || tabuleiro[linha, coluna] != 0)
                            return false;
                    }
                }
            }
            return true;
        }

        static void FixarPeca(int[,] peca, int linhaAtual, int colunaAtual, int[,] tabuleiro)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (peca[i, j] == 1)
                    {
                        int linha = linhaAtual + i;
                        int coluna = colunaAtual + j;

                        if (linha < 20 && coluna < 10)
                            tabuleiro[linha, coluna] = 1;
                    }
                }
            }
        }

        static int RemoverLinhas(int[,] tabuleiro)
        {
            int linhasRemovidas = 0;

            for (int i = 0; i < 20; i++)
            {
                bool linhaCheia = true;
                for (int j = 0; j < 10; j++)
                {
                    if (tabuleiro[i, j] == 0)
                    {
                        linhaCheia = false;
                        break;
                    }
                }

                if (linhaCheia)
                {
                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 0; j < 10; j++)
                            tabuleiro[k, j] = tabuleiro[k - 1, j];
                    }

                    for (int j = 0; j < 10; j++)
                        tabuleiro[0, j] = 0;

                    linhasRemovidas++;
                }
            }

            return linhasRemovidas;
        }
    }
}

namespace TetrisGame
{
    public class Jogador
    {
        private string nome;
        private int pontuacao;

        public Jogador(string nome)
        {
            this.nome = nome;
            this.pontuacao = 0;
        }

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public int Pontuacao
        {
            get { return pontuacao; }
        }

        public void AdicionarPontos(int valor)
        {
            pontuacao += valor;
        }

        public void SalvarPontuacao(string caminho)
        {
            string linha = $"{nome};{pontuacao}";
            File.AppendAllText(caminho, linha + Environment.NewLine);
        }

        public override string ToString()
        {
            return $"Jogador: {nome} | Pontuação: {pontuacao}";
        }
    }
}


namespace TetrisGame
{
    public class Tetrominos
    {
        private int[,] matriz = new int[3, 3];
        private string formato;

        public Tetrominos(string formato)
        {
            this.formato = formato.ToUpper();

            if (formato == "T")
            {
                matriz[0, 1] = 1;
                matriz[1, 0] = 1;
                matriz[1, 1] = 1;
                matriz[1, 2] = 1;
            }
            else if (formato == "L")
            {
                matriz[0, 0] = 1;
                matriz[1, 0] = 1;
                matriz[2, 0] = 1;
                matriz[2, 1] = 1;
            }
            else if (formato == "I")
            {
                matriz[0, 1] = 1;
                matriz[1, 1] = 1;
                matriz[2, 1] = 1;
            }
        }

        public int[,] Matriz
        {
            get { return matriz; }
        }

        public string Formato => formato;

        public void RotacionarHorario()
        {
            int[,] novaMatriz = new int[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    novaMatriz[j, 2 - i] = matriz[i, j];
            matriz = novaMatriz;
        }

        public void RotacionarAntiHorario()
        {
            int[,] novaMatriz = new int[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    novaMatriz[2 - j, i] = matriz[i, j];
            matriz = novaMatriz;
        }
    }
}
