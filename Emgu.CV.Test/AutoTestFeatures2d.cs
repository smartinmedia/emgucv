//----------------------------------------------------------------------------
//  Copyright (C) 2004-2013 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Nonfree;
using Emgu.Util;
using NUnit.Framework;

namespace Emgu.CV.Test
{
   [TestFixture]
   public class AutoTestFeatures2d
   {
#if !ANDROID
      [Test]
      public void TestBrisk()
      {
         Brisk detector = new Brisk(30, 3, 1.0f);
         EmguAssert.IsTrue(TestFeature2DTracker(detector, detector), "Unable to find homography matrix");
      }
#endif
      [Test]
      public void TestSIFT()
      {
         SIFTDetector detector = new SIFTDetector();
         EmguAssert.IsTrue(TestFeature2DTracker(detector, detector), "Unable to find homography matrix");
      }

      [Test]
      public void TestDense()
      {
         DenseFeatureDetector detector = new DenseFeatureDetector(1.0f, 1, 0.1f, 6, 0, true, false); 
         SIFTDetector extractor = new SIFTDetector();
         EmguAssert.IsTrue(TestFeature2DTracker(detector, extractor), "Unable to find homography matrix");
      }

      [Test]
      public void TestSURF()
      {
         SURFDetector detector = new SURFDetector(500, false);
         EmguAssert.IsTrue(TestFeature2DTracker(detector, detector), "Unable to find homography matrix");
      }

      [Test]
      public void TestSURFBlankImage()
      {
         SURFDetector detector = new SURFDetector(500, false);
         Image<Gray, Byte> img = new Image<Gray, byte>(1024, 900);
         ImageFeature<float>[] features = detector.DetectAndCompute(img, null);
      }

      [Test]
      public void TestStar()
      {
         StarDetector keyPointDetector = new StarDetector();

         //SURFDetector descriptorGenerator = new SURFDetector(500, false);
         SIFTDetector descriptorGenerator = new SIFTDetector();

         TestFeature2DTracker(keyPointDetector, descriptorGenerator);
      }

      [Test]
      public void TestGFTTDetector()
      {
         GFTTDetector keyPointDetector = new GFTTDetector(1000, 0.01, 1, 3, false, 0.04);
         SIFTDetector descriptorGenerator = new SIFTDetector();
         TestFeature2DTracker(keyPointDetector, descriptorGenerator);
      }

      /*
      [Test]
      public void TestDenseFeatureDetector()
      {
         DenseFeatureDetector keyPointDetector = new DenseFeatureDetector(1, 1, 0.1f, 6, 0, true, false);
         SIFTDetector descriptorGenerator = new SIFTDetector();
         TestFeature2DTracker(keyPointDetector, descriptorGenerator);
      }*/

      /*
      [Test]
      public void TestLDetector()
      {
         LDetector keyPointDetector = new LDetector();
         keyPointDetector.Init();
         
         //SURFDetector descriptorGenerator = new SURFDetector(500, false);
         SIFTDetector descriptorGenerator = new SIFTDetector(4, 3, -1, SIFTDetector.AngleMode.AVERAGE_ANGLE, 0.04 / 3 / 2.0, 10.0, 3.0, true, true);

         TestFeature2DTracker(keyPointDetector, descriptorGenerator);
      }*/


      [Test]
      public void TestMSER()
      {
         MSERDetector keyPointDetector = new MSERDetector();
         SIFTDetector descriptorGenerator = new SIFTDetector();

         TestFeature2DTracker(keyPointDetector, descriptorGenerator);
      }

      /*
      [Test]
      public void TestMSERContour()
      {
         Image<Gray, Byte> image = new Image<Gray, byte>("stuff.jpg");
         MSERDetector param = new MSERDetector();

         using (MemStorage storage = new MemStorage())
         {
            Seq<Point>[] mser = param.ExtractContours(image, null, storage);
            {
               foreach (Seq<Point> region in mser)
                  image.Draw(region, new Gray(255.0), 2);
            }
         }
      }*/

      [Test]
      public void TestFAST()
      {
         FastDetector fast = new FastDetector(10, true);
         GridAdaptedFeatureDetector fastGrid = new GridAdaptedFeatureDetector(fast, 2000, 4, 4);
         BriefDescriptorExtractor brief = new BriefDescriptorExtractor(32);
         EmguAssert.IsTrue(TestFeature2DTracker<byte>(fastGrid, brief), "Unable to find homography matrix");
      }

      [Test]
      public void TestORB()
      {
         ORBDetector orb = new ORBDetector(700);
         EmguAssert.IsTrue(TestFeature2DTracker<byte>(orb, orb), "Unable to find homography matrix");
      }

      [Test]
      public void TestFreak()
      {
         FastDetector fast = new FastDetector(10, true);
         Freak freak = new Freak(true, true, 22.0f, 4);
         EmguAssert.IsTrue(TestFeature2DTracker<byte>(fast, freak), "Unable to find homography matrix");
      }

      public static bool TestFeature2DTracker<TDescriptor>(IFeatureDetector keyPointDetector, IDescriptorExtractor<Gray, TDescriptor> descriptorGenerator)
         where TDescriptor : struct
      {
         //for (int k = 0; k < 1; k++)
         {
            Feature2D<TDescriptor> feature2D = null;
            if (keyPointDetector == descriptorGenerator)
            {
               feature2D = keyPointDetector as Feature2D<TDescriptor>;
            }

            Image<Gray, Byte> modelImage = EmguAssert.LoadImage<Gray, byte>("box.png");
            //Image<Gray, Byte> modelImage = new Image<Gray, byte>("stop.jpg");
            //modelImage = modelImage.Resize(400, 400, true);

            //modelImage._EqualizeHist();

            #region extract features from the object image
            Stopwatch stopwatch = Stopwatch.StartNew();
            VectorOfKeyPoint modelKeypoints;
            Matrix<TDescriptor> modelDescriptors;
            if (feature2D != null)
            {
               modelKeypoints = new VectorOfKeyPoint();
               modelDescriptors = feature2D.DetectAndCompute(modelImage, null, modelKeypoints);
            }
            else
            {
               modelKeypoints = keyPointDetector.DetectRaw(modelImage, null);
               modelDescriptors = descriptorGenerator.Compute(modelImage, null, modelKeypoints);
            }
            stopwatch.Stop();
            EmguAssert.WriteLine(String.Format("Time to extract feature from model: {0} milli-sec", stopwatch.ElapsedMilliseconds));
            #endregion

            //Image<Gray, Byte> observedImage = new Image<Gray, byte>("traffic.jpg");
            Image<Gray, Byte> observedImage = EmguAssert.LoadImage<Gray, byte>("box_in_scene.png");
            //Image<Gray, Byte> observedImage = modelImage.Rotate(45, new Gray(0.0));
            //image = image.Resize(400, 400, true);

            //observedImage._EqualizeHist();
            #region extract features from the observed image
            stopwatch.Reset();
            stopwatch.Start();
            VectorOfKeyPoint observedKeypoints;
            Matrix<TDescriptor> observedDescriptors;
            if (feature2D != null)
            {
               observedKeypoints = new VectorOfKeyPoint();
               observedDescriptors = feature2D.DetectAndCompute(observedImage, null, observedKeypoints);
            }
            else
            {
               observedKeypoints = keyPointDetector.DetectRaw(observedImage, null);
               observedDescriptors = descriptorGenerator.Compute(observedImage, null, observedKeypoints);
            }
            stopwatch.Stop();
            EmguAssert.WriteLine(String.Format("Time to extract feature from image: {0} milli-sec", stopwatch.ElapsedMilliseconds));
            #endregion

            //Merge the object image and the observed image into one big image for display
            Image<Gray, Byte> res = modelImage.ConcateVertical(observedImage);

            Rectangle rect = modelImage.ROI;
            PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};

            HomographyMatrix homography = null;

            stopwatch.Reset();
            stopwatch.Start();

            int k = 2;
            DistanceType dt = typeof(TDescriptor) == typeof(Byte) ? DistanceType.Hamming : DistanceType.L2;
            using (Matrix<int> indices = new Matrix<int>(observedDescriptors.Rows, k))
            using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
            using (BruteForceMatcher<TDescriptor> matcher = new BruteForceMatcher<TDescriptor>(dt))
            {
               matcher.Add(modelDescriptors);
               matcher.KnnMatch(observedDescriptors, indices, dist, k, null);

               Matrix<byte> mask = new Matrix<byte>(dist.Rows, 1);
               mask.SetValue(255);
               Features2DToolbox.VoteForUniqueness(dist, 0.8, mask);

               int nonZeroCount = CvInvoke.cvCountNonZero(mask);
               if (nonZeroCount >= 4)
               {
                  nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeypoints, observedKeypoints, indices, mask, 1.5, 20);
                  if (nonZeroCount >= 4)
                     homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeypoints, observedKeypoints, indices, mask, 2);
               }
            }
            stopwatch.Stop();
            EmguAssert.WriteLine(String.Format("Time for feature matching: {0} milli-sec", stopwatch.ElapsedMilliseconds));
            if (homography != null)
            {
               PointF[] points = pts.Clone() as PointF[];
               homography.ProjectPoints(points);

               for (int i = 0; i < points.Length; i++)
                  points[i].Y += modelImage.Height;
               res.DrawPolyline(Array.ConvertAll<PointF, Point>(points, Point.Round), true, new Gray(255.0), 5);
               return true;
            }
            else
            {
               return false;
            }

            /*
            stopwatch.Reset(); stopwatch.Start();
            //set the initial region to be the whole image
            using (Image<Gray, Single> priorMask = new Image<Gray, float>(observedImage.Size))
            {
               priorMask.SetValue(1.0);
               homography = tracker.CamShiftTrack(
                  observedFeatures,
                  (RectangleF)observedImage.ROI,
                  priorMask);
            }
            Trace.WriteLine(String.Format("Time for feature tracking: {0} milli-sec", stopwatch.ElapsedMilliseconds));
            
            if (homography != null) //set the initial tracking window to be the whole image
            {
               PointF[] points = pts.Clone() as PointF[];
               homography.ProjectPoints(points);

               for (int i = 0; i < points.Length; i++)
                  points[i].Y += modelImage.Height;
               res.DrawPolyline(Array.ConvertAll<PointF, Point>(points, Point.Round), true, new Gray(255.0), 5);
               return true;
            }
            else
            {
               return false;
            }*/

         }
      }


      [Test]
      public void TestDetectorColor()
      {
         Image<Bgr, byte> box = EmguAssert.LoadImage<Bgr, byte>("box.png");
         Image<Gray, byte> gray = box.Convert<Gray, Byte>();

         SURFDetector surf = new SURFDetector(400, false);
         OpponentColorDescriptorExtractor<float> opponentSurf = new OpponentColorDescriptorExtractor<float>(surf);
         SIFTDetector sift = new SIFTDetector();
         OpponentColorDescriptorExtractor<float> opponentSift = new OpponentColorDescriptorExtractor<float>(sift);
         //using (Util.VectorOfKeyPoint kpts = surf.DetectKeyPointsRaw(gray, null))
         using (Util.VectorOfKeyPoint kpts = sift.DetectRaw(gray, null))
         {
            for (int i = 1; i < 2; i++)
            {
               using (Matrix<float> surfDescriptors = opponentSurf.Compute(box, null, kpts))
                  EmguAssert.IsTrue(surfDescriptors.Width == (surf.SURFParams.Extended == 0? 64 : 128) * 3);

               //TODO: Find out why the following test fails
               using (Matrix<float> siftDescriptors = sift.Compute(gray, null, kpts))
                  EmguAssert.IsTrue(siftDescriptors.Width == sift.GetDescriptorSize());

               int siftDescriptorSize = sift.GetDescriptorSize();
               using (Matrix<float> siftDescriptors = opponentSift.Compute(box, null, kpts))
                  EmguAssert.IsTrue(siftDescriptors.Width == siftDescriptorSize * 3);
            }
         }
      }

      [Test]
      public void TestSURFDetector2()
      {
         //Trace.WriteLine("Size of MCvSURFParams: " + Marshal.SizeOf(typeof(MCvSURFParams)));
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         SURFDetector detector = new SURFDetector(400, false);

         Stopwatch watch = Stopwatch.StartNew();
         ImageFeature<float>[] features1 = detector.DetectAndCompute(box, null);
         watch.Stop();
         EmguAssert.WriteLine(String.Format("Time used: {0} milliseconds.", watch.ElapsedMilliseconds));

         watch.Reset();
         watch.Start();
         MKeyPoint[] keypoints = detector.Detect(box, null);
         ImageFeature<float>[] features2 = detector.Compute(box, null, keypoints);
         watch.Stop();
         EmguAssert.WriteLine(String.Format("Time used: {0} milliseconds.", watch.ElapsedMilliseconds));

         watch.Reset();
         watch.Start();
         MCvSURFParams p = detector.SURFParams;
         SURFFeature[] features3 = box.ExtractSURF(ref p);
         watch.Stop();
         EmguAssert.WriteLine(String.Format("Time used: {0} milliseconds.", watch.ElapsedMilliseconds));

         EmguAssert.IsTrue(features1.Length == features2.Length);
         EmguAssert.IsTrue(features2.Length == features3.Length);

         PointF[] pts = Array.ConvertAll<MKeyPoint, PointF>(keypoints, delegate(MKeyPoint mkp)
         {
            return mkp.Point;
         });
         //SURFFeature[] features = box.ExtractSURF(pts, null, ref detector);
         //int count = features.Length;

         /*
         for (int i = 0; i < features1.Length; i++)
         {
            Assert.AreEqual(features1[i].KeyPoint.Point, features2[i].KeyPoint.Point);
            float[] d1 = features1[i].Descriptor;
            float[] d2 = features2[i].Descriptor;

            for (int j = 0; j < d1.Length; j++)
               Assert.AreEqual(d1[j], d2[j]);
         }*/

         foreach (MKeyPoint kp in keypoints)
         {
            box.Draw(new CircleF(kp.Point, kp.Size), new Gray(255), 1);
         }
      }

      [Test]
      public void TestGridAdaptedFeatureDetectorRepeatedRun()
      {
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         SURFDetector surfdetector = new SURFDetector(400, false);

         GridAdaptedFeatureDetector detector = new GridAdaptedFeatureDetector(surfdetector, 1000, 2, 2);
         VectorOfKeyPoint kpts1 = detector.DetectRaw(box, null);
         VectorOfKeyPoint kpts2 = detector.DetectRaw(box, null);
         EmguAssert.IsTrue(kpts1.Size == kpts2.Size);
      }

      [Test]
      public void TestSURFDetectorRepeatedRun()
      {
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         SURFDetector detector = new SURFDetector(400, false);
         Image<Gray, Byte> boxInScene = EmguAssert.LoadImage<Gray, byte>("box_in_scene.png");

         ImageFeature<float>[] features1 = detector.DetectAndCompute(box, null);
         Features2DTracker<float> tracker = new Features2DTracker<float>(features1);

         ImageFeature<float>[] imageFeatures = detector.DetectAndCompute(boxInScene, null);
         Features2DTracker<float>.MatchedImageFeature[] matchedFeatures = tracker.MatchFeature(imageFeatures, 2);
         int length1 = matchedFeatures.Length;
         matchedFeatures = Features2DTracker<float>.VoteForUniqueness(matchedFeatures, 0.8);
         int length2 = matchedFeatures.Length;
         matchedFeatures = Features2DTracker<float>.VoteForSizeAndOrientation(matchedFeatures, 1.5, 20);
         int length3 = matchedFeatures.Length;

         for (int i = 0; i < 100; i++)
         {
            Features2DTracker<float>.MatchedImageFeature[] matchedFeaturesNew = tracker.MatchFeature(imageFeatures, 2);
            EmguAssert.IsTrue(length1 == matchedFeaturesNew.Length, String.Format("Failed in iteration {0}", i));
            /*
            for (int j = 0; j < length1; j++)
            {
               Features2DTracker.MatchedImageFeature oldMF = matchedFeatures[j];
               Features2DTracker.MatchedImageFeature newMF = matchedFeaturesNew[j];
               for (int k = 0; k < oldMF.SimilarFeatures.Length; k++)
               {
                  Assert.AreEqual(oldMF.SimilarFeatures[k].Distance, newMF.SimilarFeatures[k].Distance, String.Format("Failed in iteration {0}", i)); 
               }
            }*/
            matchedFeaturesNew = Features2DTracker<float>.VoteForUniqueness(matchedFeaturesNew, 0.8);
            EmguAssert.IsTrue(length2 == matchedFeaturesNew.Length, String.Format("Failed in iteration {0}", i));
            matchedFeaturesNew = Features2DTracker<float>.VoteForSizeAndOrientation(matchedFeaturesNew, 1.5, 20);
            EmguAssert.IsTrue(length3 == matchedFeaturesNew.Length, String.Format("Failed in iteration {0}", i));
         }
      }

      [Test]
      public void TestSelfMatch()
      {
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         SURFDetector surfDetector = new SURFDetector(300, false);
         ImageFeature<float>[] features1 = surfDetector.DetectAndCompute(box, null);
         Features2DTracker<float> tracker = new Features2DTracker<float>(features1);
         HomographyMatrix m = tracker.Detect(features1, 0.8);
      }

      [Test]
      public void TestLDetectorAndSelfSimDescriptor()
      {
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         LDetector detector = new LDetector();
         detector.Init();

         MKeyPoint[] keypoints = detector.DetectKeyPoints(box, 200, true);

         Point[] pts = Array.ConvertAll<MKeyPoint, Point>(keypoints, delegate(MKeyPoint k)
         {
            return Point.Round(k.Point);
         });

         SelfSimDescriptor descriptor = new SelfSimDescriptor(5, 41, 3, 7, 20);
         int descriptorSize = descriptor.DescriptorSize;

         float[] descriptors = descriptor.Compute(box, new Size(20, 20), pts);

         float absSum = 0;
         foreach (float f in descriptors)
            absSum += Math.Abs(f);

         EmguAssert.IsTrue(0 != absSum, "The sum of the descriptor should not be zero");

         EmguAssert.IsTrue(descriptors.Length / descriptor.DescriptorSize == pts.Length);

         foreach (MKeyPoint kp in keypoints)
         {
            box.Draw(new CircleF(kp.Point, kp.Size), new Gray(255), 1);
         }
      }

      [Test]
      public void TestBOWKmeansTrainer()
      {
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         SURFDetector detector = new SURFDetector(500, false);
         VectorOfKeyPoint kpts = new VectorOfKeyPoint();
         Matrix<float> descriptors = detector.DetectAndCompute(box, null, kpts);

         BOWKMeansTrainer trainer = new BOWKMeansTrainer(100, new MCvTermCriteria(), 3, CvEnum.KMeansInitType.PPCenters);
         trainer.Add(descriptors);
         Matrix<float> vocabulary = trainer.Cluster();

         BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);

         BOWImgDescriptorExtractor<float> extractor = new BOWImgDescriptorExtractor<float>(detector, matcher);
         extractor.SetVocabulary(vocabulary);

         Matrix<float> d = extractor.Compute(box, kpts);
      }

      [Test]
      public void TestBOWKmeansTrainer2()
      {
         Image<Gray, byte> box = EmguAssert.LoadImage<Gray, byte>("box.png");
         Brisk detector = new Brisk(30, 3, 1.0f);
         VectorOfKeyPoint kpts = new VectorOfKeyPoint();
         Matrix<byte> descriptors = detector.DetectAndCompute(box, null, kpts);
         Matrix<float> descriptorsF = descriptors.Convert<float>();
         BOWKMeansTrainer trainer = new BOWKMeansTrainer(100, new MCvTermCriteria(), 3, CvEnum.KMeansInitType.PPCenters);
         trainer.Add(descriptorsF);
         Matrix<float> vocabulary = trainer.Cluster();

         BruteForceMatcher<byte> matcher = new BruteForceMatcher<byte>(DistanceType.L2);

         BOWImgDescriptorExtractor<byte> extractor = new BOWImgDescriptorExtractor<byte>(detector, matcher);
         Matrix<Byte> vocabularyByte = vocabulary.Convert<Byte>();
         extractor.SetVocabulary(vocabularyByte);

         Matrix<float> d = extractor.Compute(box, kpts);
      }
   }
}
