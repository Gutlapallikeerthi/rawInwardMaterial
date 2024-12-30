-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: update_db
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `rawinwardmaterial`
--

DROP TABLE IF EXISTS `rawinwardmaterial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rawinwardmaterial` (
  `InMatId` int NOT NULL AUTO_INCREMENT,
  `InvNo` varchar(50) DEFAULT NULL,
  `InvDate` datetime DEFAULT NULL,
  `VendId` int DEFAULT '0',
  `POId` int DEFAULT '0',
  `GRNNo` varchar(50) DEFAULT NULL,
  `GRNDate` datetime DEFAULT NULL,
  `StoreId` int DEFAULT '0',
  `StoreAddId` int DEFAULT '0',
  PRIMARY KEY (`InMatId`),
  UNIQUE KEY `InMatId_UNIQUE` (`InMatId`)
) ENGINE=InnoDB AUTO_INCREMENT=100000057 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rawinwardmaterial`
--

LOCK TABLES `rawinwardmaterial` WRITE;
/*!40000 ALTER TABLE `rawinwardmaterial` DISABLE KEYS */;
INSERT INTO `rawinwardmaterial` VALUES (100000050,'string','2024-12-27 07:34:31',0,0,'string','2024-12-27 07:34:31',0,0),(100000051,'string','2024-12-27 07:34:31',0,0,'string','2024-12-27 07:34:31',0,0),(100000052,'string','2024-12-27 07:34:31',0,0,'string','2024-12-27 07:34:31',0,0),(100000053,'string','2024-12-27 11:21:28',0,0,'string','2024-12-27 11:21:28',0,0),(100000054,'string','2024-12-27 11:22:50',0,0,'string','2024-12-27 11:22:50',0,0),(100000056,'string','2024-12-30 04:48:44',0,0,'string','2024-12-30 04:48:44',0,0);
/*!40000 ALTER TABLE `rawinwardmaterial` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rawinwardmaterialsub`
--

DROP TABLE IF EXISTS `rawinwardmaterialsub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rawinwardmaterialsub` (
  `InMatSubId` int NOT NULL AUTO_INCREMENT,
  `InMatId` int DEFAULT NULL,
  `SlNo` int DEFAULT NULL,
  `ItemId` int DEFAULT NULL,
  `Qty` double DEFAULT NULL,
  `NoOfBags` int DEFAULT '0',
  `BatchNo` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`InMatSubId`),
  UNIQUE KEY `InMatSubId_UNIQUE` (`InMatSubId`),
  KEY `fk_RawInwardMaterial_idx` (`InMatId`),
  CONSTRAINT `fk_RawInwardMaterial` FOREIGN KEY (`InMatId`) REFERENCES `rawinwardmaterial` (`InMatId`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=88 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rawinwardmaterialsub`
--

LOCK TABLES `rawinwardmaterialsub` WRITE;
/*!40000 ALTER TABLE `rawinwardmaterialsub` DISABLE KEYS */;
INSERT INTO `rawinwardmaterialsub` VALUES (80,100000050,0,0,0,0,'string'),(81,100000051,0,0,0,0,'string'),(82,100000052,0,0,0,0,'string'),(83,100000053,0,0,0,0,'string'),(84,100000054,0,0,0,0,'string'),(87,100000056,0,0,0,0,'string');
/*!40000 ALTER TABLE `rawinwardmaterialsub` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `storeadd`
--

DROP TABLE IF EXISTS `storeadd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `storeadd` (
  `StoreAddId` int NOT NULL AUTO_INCREMENT,
  `InMatId` int DEFAULT NULL,
  `StoreAddNo` int DEFAULT '0',
  `StoreAddDate` datetime DEFAULT NULL,
  `Source` int DEFAULT '0' COMMENT '1-RM Inward\n2-PM Inward\n3-FG Inward',
  `StoreId` int DEFAULT '0',
  `RefDocId` int DEFAULT '0',
  PRIMARY KEY (`StoreAddId`),
  UNIQUE KEY `StoreAddId_UNIQUE` (`StoreAddId`),
  KEY `FK_RawInward_idx` (`InMatId`),
  CONSTRAINT `FK_RawInward` FOREIGN KEY (`InMatId`) REFERENCES `rawinwardmaterial` (`InMatId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_storeadd_inMatId` FOREIGN KEY (`InMatId`) REFERENCES `rawinwardmaterial` (`InMatId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=76 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storeadd`
--

LOCK TABLES `storeadd` WRITE;
/*!40000 ALTER TABLE `storeadd` DISABLE KEYS */;
INSERT INTO `storeadd` VALUES (1,100000050,0,'2024-12-27 07:34:31',0,0,0),(2,100000051,0,'2024-12-27 07:34:31',0,0,0),(3,100000052,0,'2024-12-27 07:34:31',0,0,0),(71,100000053,0,'2024-12-27 11:21:28',0,0,0),(72,100000054,0,'2024-12-27 11:22:50',0,0,0),(75,100000056,0,'2024-12-30 04:48:44',0,0,0);
/*!40000 ALTER TABLE `storeadd` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `storeaddsub`
--

DROP TABLE IF EXISTS `storeaddsub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `storeaddsub` (
  `storeAddSubId` int NOT NULL AUTO_INCREMENT,
  `StoreAddId` int DEFAULT NULL,
  `SlNo` int DEFAULT '0',
  `ItemId` int DEFAULT NULL,
  `Qty` float DEFAULT '0',
  `BalQty` float DEFAULT '0',
  `BagNo` int DEFAULT '0',
  `BatchNo` varchar(255) DEFAULT NULL,
  `Trak` longtext,
  PRIMARY KEY (`storeAddSubId`),
  UNIQUE KEY `storeAddSubId_UNIQUE` (`storeAddSubId`),
  KEY `FK_STOREADD_idx` (`StoreAddId`),
  CONSTRAINT `FK_STOREADD` FOREIGN KEY (`StoreAddId`) REFERENCES `storeadd` (`StoreAddId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=75 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storeaddsub`
--

LOCK TABLES `storeaddsub` WRITE;
/*!40000 ALTER TABLE `storeaddsub` DISABLE KEYS */;
INSERT INTO `storeaddsub` VALUES (68,1,0,0,0,0,0,'string','string'),(69,2,0,0,0,0,0,'string','string'),(70,3,0,0,0,0,0,'string','string'),(71,71,0,0,0,0,0,'string','string'),(72,72,0,0,0,0,0,'string','string'),(74,75,0,0,0,0,0,'string','string');
/*!40000 ALTER TABLE `storeaddsub` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-12-30 13:36:04
