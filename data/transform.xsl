<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<title>Ресурси кафедри</title>
			</head>
			<body>
				<h1>Список ресурсів</h1>
				<table border="1">
					<tr>
						<th>Анотація</th>
						<th>Автор</th>
						<th>Умови використання</th>
						<th>Адреса</th>
					</tr>
					<xsl:for-each select="//resource">
						<tr>
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
