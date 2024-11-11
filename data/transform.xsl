<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/resources">
		<html>
			<head>
				<title>Ресурси кафедри</title>
			</head>
			<body>
				<h2>Інформаційні ресурси кафедри</h2>
				<table border="1">
					<tr>
						<th>Назва</th>
						<th>Тип</th>
						<th>Анотація</th>
						<th>Автор</th>
						<th>Умови використання</th>
						<th>Адреса</th>
					</tr>
					<xsl:for-each select="resource">
						<tr>
							<td>
								<xsl:value-of select="@title"/>
							</td>
							<td>
								<xsl:value-of select="@type"/>
							</td>
							<td>
								<xsl:value-of select="annotation"/>
							</td>
							<td>
								<xsl:value-of select="author"/>
							</td>
							<td>
								<xsl:value-of select="usage_conditions"/>
							</td>
							<td>
								<xsl:value-of select="address"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
