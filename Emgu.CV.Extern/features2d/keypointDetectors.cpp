//----------------------------------------------------------------------------
//
//  Copyright (C) 2004-2013 by EMGU. All rights reserved.
//
//----------------------------------------------------------------------------

#include "features2d_c.h"

//StarDetector
cv::StarDetector* CvStarDetectorCreate(int maxSize, int responseThreshold, int lineThresholdProjected, int lineThresholdBinarized, int suppressNonmaxSize)
{
   return new cv::StarDetector(maxSize, responseThreshold, lineThresholdProjected, lineThresholdBinarized, suppressNonmaxSize);
}

void CvStarDetectorRelease(cv::StarDetector** detector)
{
   delete *detector;
   *detector = 0;
}

//ORB
cv::ORB* CvOrbDetectorCreate(int numberOfFeatures, float scaleFactor, int nLevels, int edgeThreshold, int firstLevel, int WTA_K, int scoreType, int patchSize, cv::FeatureDetector** featureDetector, cv::DescriptorExtractor** descriptorExtractor)
{
   cv::ORB* orb = new cv::ORB(numberOfFeatures, scaleFactor, nLevels, edgeThreshold, firstLevel, WTA_K, scoreType, patchSize);
   *featureDetector = static_cast<cv::FeatureDetector*>(orb);
   *descriptorExtractor = static_cast<cv::DescriptorExtractor*>(orb);
   return orb;
}

/*
int CvOrbDetectorGetDescriptorSize(cv::ORB* detector)
{
   return detector->descriptorSize();
}

void CvOrbDetectorComputeDescriptors(cv::ORB* detector, IplImage* image, std::vector<cv::KeyPoint>* keypoints, CvMat* descriptors)
{
   if (keypoints->size() <= 0) return;
   cv::Mat mat = cv::cvarrToMat(image);
   cv::Mat maskMat;

   cv::Mat descriptorsMat = cv::cvarrToMat(descriptors);
   if (mat.channels() == 1)
   {
      (*detector)(mat, maskMat, *keypoints, descriptorsMat, true);
   } else
   {
      //TODO: create the extractor using the parameters in the ORB detector instead of using the default values
      cv::Ptr<cv::DescriptorExtractor> extractor = new cv::OrbDescriptorExtractor();

      cv::OpponentColorDescriptorExtractor colorExtractor(extractor);
      colorExtractor.compute(mat, *keypoints, descriptorsMat);
   }
}*/

void CvOrbDetectorRelease(cv::ORB** detector)
{
   delete *detector;
   *detector = 0;
}

//FREAK
cv::FREAK* CvFreakCreate(bool orientationNormalized, bool scaleNormalized, float patternScale, int nOctaves)
{
   return new cv::FREAK(orientationNormalized, scaleNormalized, patternScale, nOctaves);
}

void CvFreakRelease(cv::FREAK** detector)
{
   delete * detector;
   *detector = 0;
}

//Brisk
cv::BRISK* CvBriskCreate(int thresh, int octaves, float patternScale, cv::FeatureDetector** featureDetector, cv::DescriptorExtractor** descriptorExtractor)
{
   cv::BRISK* brisk = new cv::BRISK(thresh, octaves, patternScale);
   *featureDetector = static_cast<cv::FeatureDetector*>(brisk);
   *descriptorExtractor = static_cast<cv::DescriptorExtractor*>(brisk);
   return brisk;
}

void CvBriskRelease(cv::BRISK** detector)
{
   delete *detector;
   *detector = 0;
}

//FeatureDetector
void CvFeatureDetectorDetectKeyPoints(cv::FeatureDetector* detector, IplImage* image, IplImage* mask, std::vector<cv::KeyPoint>* keypoints)
{
   cv::Mat mat = cv::cvarrToMat(image);
   cv::Mat maskMat;
   if (mask) maskMat = cv::cvarrToMat(mask);
   detector->detect(mat, *keypoints, maskMat);
}

void CvFeatureDetectorRelease(cv::FeatureDetector** detector)
{
   delete *detector;
   *detector = 0;
}

//GridAdaptedFeatureDetector
cv::GridAdaptedFeatureDetector* GridAdaptedFeatureDetectorCreate(   
   cv::FeatureDetector* detector,
   int maxTotalKeypoints,
   int gridRows, int gridCols)
{
   cv::Ptr<cv::FeatureDetector> detectorPtr(detector);
   detectorPtr.addref(); //increment the counter such that it should never be release by the grid adapeted feature detector
   return new cv::GridAdaptedFeatureDetector(detectorPtr, maxTotalKeypoints, gridRows, gridCols);
}
/*
void GridAdaptedFeatureDetectorDetect(
   cv::GridAdaptedFeatureDetector* detector, 
   const cv::Mat* image, std::vector<cv::KeyPoint>* keypoints, const cv::Mat* mask)
{
   cv::Mat mat = cv::cvarrToMat(image);
   cv::Mat maskMat = mask? cv::cvarrToMat(mask) : cv::Mat();
   detector->detect(mat, *keypoints, maskMat);
}*/

void GridAdaptedFeatureDetectorRelease(cv::GridAdaptedFeatureDetector** detector)
{
   delete *detector;
   *detector = 0;
}

cv::BriefDescriptorExtractor* CvBriefDescriptorExtractorCreate(int descriptorSize)
{
   return new cv::BriefDescriptorExtractor(descriptorSize);
}

/*
int CvBriefDescriptorExtractorGetDescriptorSize(cv::BriefDescriptorExtractor* extractor)
{
   return extractor->descriptorSize();
}

void CvBriefDescriptorComputeDescriptors(cv::BriefDescriptorExtractor* extractor, IplImage* image, std::vector<cv::KeyPoint>* keypoints, cv::Mat* descriptors)
{
   if (keypoints->size() <= 0) return;
   cv::Mat img = cv::cvarrToMat(image);
   if (img.channels() == 1)
   {
     extractor->compute(img, *keypoints, *descriptors);
   } else //opponent brief
   {
      cv::Ptr<cv::DescriptorExtractor> briefExtractor = new cv::BriefDescriptorExtractor(extractor->descriptorSize());
      cv::OpponentColorDescriptorExtractor colorDescriptorExtractor(briefExtractor);
      colorDescriptorExtractor.compute(img, *keypoints, *descriptors);
   }
}*/

void CvBriefDescriptorExtractorRelease(cv::BriefDescriptorExtractor** extractor)
{
   delete *extractor;
   *extractor = 0;
}

// detect corners using FAST algorithm
cv::FastFeatureDetector* CvFASTGetFeatureDetector(int threshold, bool nonmax_supression)
{
   return new cv::FastFeatureDetector(threshold, nonmax_supression);
}

void CvFASTFeatureDetectorRelease(cv::FastFeatureDetector** detector)
{
   delete *detector;
   *detector = 0;
}

// MSER detector
cv::MSER* CvMserGetFeatureDetector(CvMSERParams* detector)
{  
   return new cv::MSER(
      detector->delta,
      detector->minArea, 
      detector->maxArea,
      detector->maxVariation,
      detector->minDiversity, 
      detector->maxEvolution,
      detector->areaThreshold,
      detector->minMargin, 
      detector->edgeBlurSize);
}

void CvMserFeatureDetectorRelease(cv::MSER** detector)
{
   delete *detector;
   *detector = 0;
}

// SimpleBlobDetector
cv::SimpleBlobDetector* CvSimpleBlobDetectorCreate()
{
   return new cv::SimpleBlobDetector();
}
void CvSimpleBlobDetectorRelease(cv::SimpleBlobDetector** detector)
{
   delete *detector;
   *detector = 0;
}

// Draw keypoints.
void drawKeypoints(
                          const IplImage* image, 
                          const std::vector<cv::KeyPoint>* keypoints, 
                          IplImage* outImage,
                          const CvScalar color, 
                          int flags)
{
   cv::Mat mat = cv::cvarrToMat(image);
   cv::Mat outMat = cv::cvarrToMat(outImage);
   cv::drawKeypoints(mat, *keypoints, outMat, color, flags);
}

// Draws matches of keypints from two images on output image.
void drawMatchedFeatures(
                                const IplImage* img1, const std::vector<cv::KeyPoint>* keypoints1,
                                const IplImage* img2, const std::vector<cv::KeyPoint>* keypoints2,
                                const CvMat* matchIndices, 
                                IplImage* outImg,
                                const CvScalar matchColor, const CvScalar singlePointColor,
                                const CvMat* matchesMask, 
                                int flags)
{
   cv::Mat mat1 = cv::cvarrToMat(img1);
   cv::Mat mat2 = cv::cvarrToMat(img2);

   std::vector<cv::DMatch> matches;
   VectorOfDMatchPushMatrix(&matches, matchIndices, 0, matchesMask);

   cv::Mat outMat = cv::cvarrToMat(outImg);
   cv::drawMatches(mat1, *keypoints1, mat2, *keypoints2, matches, outMat, 
      matchColor, singlePointColor, std::vector<char>(), flags);
}

//DescriptorMatcher
void CvDescriptorMatcherAdd(cv::DescriptorMatcher* matcher, CvMat* trainDescriptor)
{
   cv::Mat trainMat = cv::cvarrToMat(trainDescriptor);
   std::vector<cv::Mat> trainVector;
   trainVector.push_back(trainMat);
   matcher->add(trainVector);   
}

void CvDescriptorMatcherKnnMatch(cv::DescriptorMatcher* matcher, const CvMat* queryDescriptors, 
                   CvMat* trainIdx, CvMat* distance, int k,
                   const CvMat* mask) 
{
   std::vector< std::vector< cv::DMatch > > matches; //The first index is the index of the query

   //only implemented for a single trained image for now
   CV_Assert( matcher->getTrainDescriptors().size() == 1);

   cv::Mat queryMat = cv::cvarrToMat(queryDescriptors);
   cv::Mat maskMat = mask ? cv::cvarrToMat(mask) : cv::Mat();
   std::vector<cv::Mat> masks;
   if (!maskMat.empty()) 
      masks.push_back(maskMat);

   matcher->knnMatch(queryMat, matches, k, masks, false);
   
   VectorOfDMatchToMat(&matches, trainIdx, distance);
}

cv::DescriptorMatcher* CvBruteForceMatcherCreate(int distanceType, bool crossCheck)
{
   return new cv::BFMatcher(distanceType, crossCheck);
}

void CvBruteForceMatcherRelease(cv::DescriptorMatcher** matcher)
{
   delete *matcher;
   *matcher = 0;
}

//2D tracker
int voteForSizeAndOrientation(std::vector<cv::KeyPoint>* modelKeyPoints, std::vector<cv::KeyPoint>* observedKeyPoints, CvArr* indices, CvArr* mask, double scaleIncrement, int rotationBins)
{
   CV_Assert(!modelKeyPoints->empty());
   CV_Assert(!observedKeyPoints->empty());
   cv::Mat_<int> indicesMat = (cv::Mat_<int>) cv::cvarrToMat(indices);
   cv::Mat_<uchar> maskMat = (cv::Mat_<uchar>) cv::cvarrToMat(mask);
   std::vector<float> logScale;
   std::vector<float> rotations;
   float s, maxS, minS, r;
   maxS = -1.0e-10f; minS = 1.0e10f;

   for (int i = 0; i < maskMat.rows; i++)
   {
      if ( maskMat(i, 0)) 
      {
         cv::KeyPoint observedKeyPoint = observedKeyPoints->at(i);
         cv::KeyPoint modelKeyPoint = modelKeyPoints->at( indicesMat(i, 0));
         s = log10( observedKeyPoint.size / modelKeyPoint.size );
         logScale.push_back(s);
         maxS = s > maxS ? s : maxS;
         minS = s < minS ? s : minS;

         r = observedKeyPoint.angle - modelKeyPoint.angle;
         r = r < 0.0f? r + 360.0f : r;
         rotations.push_back(r);
      }    
   }

   int scaleBinSize = cvCeil((maxS - minS) / log10(scaleIncrement));
   if (scaleBinSize < 2) scaleBinSize = 2;
   float scaleRanges[] = {minS, (float) (minS + scaleBinSize * log10(scaleIncrement))};

   cv::Mat_<float> scalesMat(logScale);
   cv::Mat_<float> rotationsMat(rotations);
   std::vector<float> flags(logScale.size());
   cv::Mat flagsMat(flags);

   {  //Perform voting for both scale and orientation
      int histSize[] = {scaleBinSize, rotationBins};
      float rotationRanges[] = {0, 360};
      int channels[] = {0, 1};
      const float* ranges[] = {scaleRanges, rotationRanges};
      double minVal, maxVal;

      const cv::Mat_<float> arrs[] = {scalesMat, rotationsMat}; 

      cv::MatND hist; //CV_32S
      cv::calcHist(arrs, 2, channels, cv::Mat(), hist, 2, histSize, ranges, true);
      cv::minMaxLoc(hist, &minVal, &maxVal);

      cv::threshold(hist, hist, maxVal * 0.5, 0, cv::THRESH_TOZERO);
      cv::calcBackProject(arrs, 2, channels, hist, flagsMat, ranges);
   }

   int idx =0;
   int nonZeroCount = 0;
   for (int i = 0; i < maskMat.rows; i++)
   {
      if (maskMat(i, 0))
      {
         if (flags[idx++] != 0.0f)
            nonZeroCount++;
         else 
            maskMat(i, 0) = 0;
      }
   }
   return nonZeroCount;
}

void CvFeature2DDetectAndCompute(cv::Feature2D* feature2D, IplImage* image, IplImage* mask, std::vector<cv::KeyPoint>* keypoints, cv::Mat* descriptors, bool useProvidedKeyPoints)
{
   cv::Mat imageMat = cv::cvarrToMat(image);
   cv::Mat maskMat = mask ? cv::cvarrToMat(mask) : cv::Mat();
   (*feature2D)(imageMat, maskMat, *keypoints, *descriptors, useProvidedKeyPoints);
}

//OpponentColorDescriptorExtractor
cv::OpponentColorDescriptorExtractor* CvOpponentColorDescriptorExtractorCreate(cv::DescriptorExtractor* extractor)
{
   cv::Ptr<cv::DescriptorExtractor> ptr(extractor);
   ptr.addref();
   return new cv::OpponentColorDescriptorExtractor(ptr);
}
void CvOpponentColorDescriptorExtractorRelease(cv::OpponentColorDescriptorExtractor** extractor)
{
   delete *extractor;
   *extractor = 0;
}

//DescriptorExtractor
void CvDescriptorExtractorCompute(cv::DescriptorExtractor* extractor, const IplImage* image,  std::vector<cv::KeyPoint>* keypoints, cv::Mat* descriptors )
{
   cv::Mat imageMat = cv::cvarrToMat(image);
   extractor->compute(imageMat, *keypoints, *descriptors);
}

int CvDescriptorExtractorGetDescriptorSize(cv::DescriptorExtractor* extractor)
{
   return extractor->descriptorSize();
}

//GFTT
cv::GFTTDetector* CvGFTTDetectorCreate( int maxCorners, double qualityLevel, double minDistance, int blockSize, bool useHarrisDetector, double k)
{
   return new cv::GFTTDetector(maxCorners, qualityLevel, minDistance, blockSize, useHarrisDetector, k);
}
void CvGFTTDetectorRelease(cv::GFTTDetector** detector)
{
   delete *detector;
   *detector = 0;
}

//DenseFeatureDetector
cv::DenseFeatureDetector* CvDenseFeatureDetectorCreate( float initFeatureScale, int featureScaleLevels, float featureScaleMul, int initXyStep, int initImgBound, bool varyXyStepWithScale, bool varyImgBoundWithScale)
{
   return new cv::DenseFeatureDetector(initFeatureScale, featureScaleLevels, featureScaleMul, initXyStep, initImgBound, varyXyStepWithScale, varyImgBoundWithScale);
}
void CvDenseFeatureDetectorRelease(cv::DenseFeatureDetector** detector)
{
   delete * detector;
   *detector = 0;
}

//BowKMeansTrainer
cv::BOWKMeansTrainer* CvBOWKMeansTrainerCreate(int clusterCount, const CvTermCriteria termcrit, int attempts, int flags)
{
   return new cv::BOWKMeansTrainer(clusterCount, termcrit, attempts, flags);
}
void CvBOWKMeansTrainerRelease(cv::BOWKMeansTrainer** trainer)
{
   delete * trainer;
   *trainer = 0;
}
int CvBOWKMeansTrainerGetDescriptorCount(cv::BOWKMeansTrainer* trainer)
{
   return trainer->descripotorsCount();
}
void CvBOWKMeansTrainerAdd(cv::BOWKMeansTrainer* trainer, CvMat* descriptors)
{
   cv::Mat m = cv::cvarrToMat(descriptors);
   trainer->add(m);
}
void CvBOWKMeansTrainerCluster(cv::BOWKMeansTrainer* trainer, cv::Mat* descriptors)
{
   cv::Mat m = trainer->cluster();
   cv::swap(m, *descriptors);
}

//BOWImgDescriptorExtractor
cv::BOWImgDescriptorExtractor* CvBOWImgDescriptorExtractorCreate(cv::DescriptorExtractor* descriptorExtractor, cv::DescriptorMatcher* descriptorMatcher)
{
      cv::Ptr<cv::DescriptorExtractor> extractorPtr(descriptorExtractor);
   extractorPtr.addref();
         cv::Ptr<cv::DescriptorMatcher> matcherPtr(descriptorMatcher);
   matcherPtr.addref();
   return new cv::BOWImgDescriptorExtractor(extractorPtr, matcherPtr);
}
void CvBOWImgDescriptorExtractorRelease(cv::BOWImgDescriptorExtractor** descriptorExtractor)
{
   delete *descriptorExtractor;
   *descriptorExtractor = 0;
}
void CvBOWImgDescriptorExtractorSetVocabulary(cv::BOWImgDescriptorExtractor* bowImgDescriptorExtractor, CvMat* vocabulary)
{
   cv::Mat voc = cv::cvarrToMat(vocabulary);
   bowImgDescriptorExtractor->setVocabulary(voc);
}
void CvBOWImgDescriptorExtractorCompute(cv::BOWImgDescriptorExtractor* bowImgDescriptorExtractor, const IplImage* image, std::vector<cv::KeyPoint>* keypoints, cv::Mat* imgDescriptor)
{
   cv::Mat img = cv::cvarrToMat(image);
   bowImgDescriptorExtractor->compute(img, *keypoints, *imgDescriptor);
}

