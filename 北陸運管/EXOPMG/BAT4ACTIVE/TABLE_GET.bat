BCP "select o.name from %2.dbo.sysindexes i, %2.dbo.sysobjects o where o.xtype='U' and o.id=i.id and i.indid<2" queryout ".\DATA_LIST.txt" -c -t -S %1 -U %3 -P %4
