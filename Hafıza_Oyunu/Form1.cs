namespace Hafıza_Oyunu
{
    public partial class Form1 : Form
    {

        List<PictureBox> pictureBoxes = new List<PictureBox>();
        List<Image> images = new List<Image>(); // Resimlerimizi bu listede tutacağız
        PictureBox firstClicked = null, secondClicked = null;
        int player1Score = 0, player2Score = 0;
        int matchedPairs = 0; // Eşleşen çift sayısı
        bool isPlayer1Turn = true;
        // Timer revealTimer;
        int countdownTime = 5; // Geri sayım süresi, başlangıç değeri 5 saniye


        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            // InitializeTimer(); // Timer'ı burada başlat

        }

        private void InitializeGame()
        {
            // Resimleri PictureBox'lara ekle
            for (int i = 1; i <= 40; i++)
            {
                PictureBox pb = (PictureBox)this.Controls.Find($"pictureBox{i}", true)[0];
                pb.Click += pictureBox_Click;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxes.Add(pb);
            }

            // Resimleri diziye ekle (her biri 2 kez olacak şekilde)
            for (int i = 1; i <= 20; i++)
            {
                // Resim yolunu oluştur
                string imagePath = $"Hafıza_Oyunu/Images/image{i}.jpg";//dosya yolunu projenin yoluna göre belirleyin

                // Resmi yükle
                Image img = Image.FromFile(imagePath);

                // İki kez ekle
                images.Add(img); // İlk kopya
                images.Add(img); // İkinci kopya
            }

            // Resimleri karıştır
            Random rnd = new Random();
            images = images.OrderBy(x => rnd.Next()).ToList();

            // Karıştırılmış resimleri PictureBox'lara yerleştir
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                pictureBoxes[i].Tag = images[i]; // Tag'de resmi tut
            }

            // Skorları sıfırla
            lblPlayer1Score.Text = "0";
            lblPlayer2Score.Text = "0";

            // 5 saniye boyunca resimleri göster
            ShowImagesFor5Seconds();
        }





        private void ShowImagesFor5Seconds()
        {
            // Tüm resimleri aç
            foreach (var pb in pictureBoxes)
            {
                pb.Image = (Image)pb.Tag; // Resmi göster
            }

            // 5 saniye sonra resimleri gizle
            timer1.Interval = 5000; // 5 saniye
            timer1.Tick += HideImages; // Timer bittiğinde resimleri gizleyecek
            timer1.Start();
        }

        private void HideImages(object sender, EventArgs e)
        {
            // Resimleri gizle
            foreach (var pb in pictureBoxes)
            {
                pb.Image = null;
            }

            // Timer'ı durdur ve Tick olayını temizle
            timer1.Stop();
            timer1.Tick -= HideImages;
        }



        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;

            // Eğer zaten bir resim tıklandıysa geri dön
            if (clickedPictureBox == null || clickedPictureBox.Image != null)
                return;

            // İlk tıklama mı?
            if (firstClicked == null)
            {
                firstClicked = clickedPictureBox;
                firstClicked.Image = (Image)firstClicked.Tag;

                // Timer2'yi başlat
                countdownTime = 5; // Geri sayımı 5 saniye olarak ayarla
                lblSure.Text = countdownTime.ToString(); // Label'ı güncelle
                timer2.Interval = 1000; // 1 saniyelik aralıklarla çalışsın
                timer2.Start();
                return;
            }

            // İkinci tıklama
            secondClicked = clickedPictureBox;
            secondClicked.Image = (Image)secondClicked.Tag;

            timer2.Stop();


            // Eğer eşleşme varsa
            if (AreImagesEqual((Image)firstClicked.Tag, (Image)secondClicked.Tag))
            {
                matchedPairs++;
                UpdateScore();

                // Eşleşme olduğunda sırayı değiştirme
                firstClicked = null;
                secondClicked = null;
            }
            else
            {
                // Eşleşme olmadı, Timer'ı başlat ve resimleri gizle
                timer1.Interval = 1000; // 1 saniye bekleme
                timer1.Tick += HidePictures;
                timer1.Start();

                // Eşleşme yoksa sırayı değiştir
                isPlayer1Turn = !isPlayer1Turn;
                UpdatePlayerTurnLabel(); // Oyuncu sırasını güncelle
            }
        }


        // Resimlerin eşit olup olmadığını kontrol eden metod
        private bool AreImagesEqual(Image img1, Image img2)
        {
            // Resimlerin referanslarının aynı olup olmadığını kontrol et
            if (img1 == img2)
            {
                return true;
            }

            // Eğer referanslar farklıysa, pixel bazlı karşılaştırma yapmak gerekir
            // Bunu yapmak için Bitmap oluşturuyoruz
            Bitmap bmp1 = new Bitmap(img1);
            Bitmap bmp2 = new Bitmap(img2);

            // Boyutlar aynı değilse zaten eşit olamazlar
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
            {
                return false;
            }

            // Her pikseli karşılaştır
            for (int x = 0; x < bmp1.Width; x++)
            {
                for (int y = 0; y < bmp1.Height; y++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void UpdateScore()
        {
            if (isPlayer1Turn)
            {
                player1Score++;
                lblPlayer1Score.Text = $"0: {player1Score}";
            }
            else
            {
                player2Score++;
                lblPlayer2Score.Text = $"O: {player2Score}";
            }

            // Tüm çiftler eşleşti mi?
            if (matchedPairs == 20) // 20 eşleşme olduğunda oyun biter
            {
                timer2.Stop(); // Timer'ı durdur
                timer1.Stop(); // Diğer timer'ları da durdur

                // Kazananı belirle
                string winnerMessage;
                if (player1Score > player2Score)
                {
                    winnerMessage = "Oyun bitti! Kazanan: Oyuncu 1";
                }
                else if (player2Score > player1Score)
                {
                    winnerMessage = "Oyun bitti! Kazanan: Oyuncu 2";
                }
                else
                {
                    winnerMessage = "Oyun bitti! Berabere!";
                }

                MessageBox.Show(winnerMessage, "Oyun Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }


        }
        private void UpdatePlayerTurnLabel()
        {
            if (isPlayer1Turn)
            {
                lbloyunSırası.Text = "1";
            }
            else
            {
                lbloyunSırası.Text = "2";
            }

           

        }

        private void HidePictures(object sender, EventArgs e)
        {
            // Eşleşmeyen resimleri gizle
            firstClicked.Image = null;
            secondClicked.Image = null;

            // Değişkenleri sıfırla
            firstClicked = null;
            secondClicked = null;

            // Timer'ı durdur ve Tick olayını temizle
            timer1.Stop();
            timer1.Tick -= HidePictures;
        }




        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Skorları sıfırla
            player1Score = 0;
            player2Score = 0;
            matchedPairs = 0;
            isPlayer1Turn = true;

            // PictureBox'ları temizle
            foreach (var pb in pictureBoxes)
            {
                pb.Image = null; // Resimleri gizle
            }

            // Oyunu yeniden başlat
            InitializeGame();
            timer2.Stop();
            lblSure.Text = ""; // Label'ı temizle



        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            countdownTime--; // Geri sayımı azalt

            // Label'ı güncelle
            lblSure.Text = countdownTime.ToString();

            // Geri sayım sıfıra ulaştıysa
            if (countdownTime <= 0)
            {
                // Timer'ı durdur
                timer2.Stop();

                // Eşleşme yoksa sırayı değiştir
                isPlayer1Turn = !isPlayer1Turn;
                UpdatePlayerTurnLabel(); // Oyuncu sırasını güncelle

                // Kartları gizle
                if (firstClicked != null)
                {
                    firstClicked.Image = null;
                }

                if (secondClicked != null)
                {
                    secondClicked.Image = null;
                }

                // Değişkenleri sıfırla
                firstClicked = null;
                secondClicked = null;

                // Geri sayım label'ını temizle
                lblSure.Text = "";
            }

        }

    }
}
