﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt"
      xmlns:utils="urn:myExtension"
     exclude-result-prefixes="msxsl utils">
  <xsl:output method="xml" indent="yes"/>

   <msxsl:script implements-prefix="utils" language="C#">
    <![CDATA[
      public string TuUpper(string stringValue)
      {
        string result = String.Empty;

        if(!String.IsNullOrEmpty(stringValue))
        {
          result = stringValue.ToUpper().Trim(); 
        }

        return result;
      }
    ]]>
  </msxsl:script>

  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/">
    <xsl:apply-templates select="/Merged/report" />
  </xsl:template>

  <xsl:template match="result">
    <xsl:copy>
      
    </xsl:copy>
  </xsl:template>

  <!-- this template is applied to a path node that doesn't have a fill attribute -->
  <xsl:template match="result[not(@ref)]">
    <!-- copy me and my attributes and my subnodes, applying templates as necessary, and add a fill attribute set to red -->
   
    <xsl:variable name="CurrentSkytter">
      <xsl:value-of select="concat(utils:TuUpper(@name),utils:TuUpper(@club),utils:TuUpper(@class))"/>
    </xsl:variable>
    
    <xsl:variable name="FoundSkyttere">
      <Hits>
         <xsl:for-each select="/Merged/Skyttere" >
            <xsl:for-each select="Skytter" >
              <xsl:variable name="CurrentRowSkytter">
                <xsl:value-of select="concat(utils:TuUpper(@name),utils:TuUpper(@club),utils:TuUpper(@class))"/>
              </xsl:variable>
             
               <xsl:if test="$CurrentSkytter = $CurrentRowSkytter">
                <Hit>
                  <xsl:copy-of select="current()"/>
                </Hit>
              </xsl:if>
            </xsl:for-each>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundSkytter">
      <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@name"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:if test="string($FoundSkytter) !=''">
        <xsl:attribute name="ref">
          <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>



</xsl:stylesheet>
