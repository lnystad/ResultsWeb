<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt"
     exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/">
    <Skyttere>
      <xsl:apply-templates select="/Merged/ArrayOfStartingListLag/StartingListLag" />
    </Skyttere>
  </xsl:template>
  
  <!--<xsl:template match="ArrayOfStartingListLag">
    <xsl:apply-templates select="/Merged/report/data/result" />
  </xsl:template>-->
  
  <xsl:template match="StartingListLag">
    <xsl:variable name ="Lagnr">
      <xsl:value-of select="LagNr"/>
    </xsl:variable>

    <xsl:for-each select="AllSkiver/Skive">
      
      
    
    <xsl:variable name ="skivenr">
      <xsl:value-of select="LagSkiveNr"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.PNG')"/>
    </xsl:variable>

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind">
            <Hit>
              <xsl:value-of select="current()"/>
            </Hit>
          </xsl:if>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundBitmap">
      <xsl:value-of select="msxsl:node-set($FoundBitmaps)/Hits/Hit"/>
    </xsl:variable>

    <!-- produce a fill attribute with content "red" -->
    <xsl:if test="string($FoundBitmap) !=''">
      <xsl:element name="Skytter">
        <xsl:attribute name="name">
          <xsl:value-of select="SkytterNavn"/>
        </xsl:attribute>
        <xsl:attribute name="club">
          <xsl:value-of select="SkytterLag"/>
        </xsl:attribute>
        <xsl:attribute name="class">
          <xsl:value-of select="Klasse"/>
        </xsl:attribute>
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:element>
    </xsl:if>
    </xsl:for-each>

  </xsl:template>




</xsl:stylesheet>
