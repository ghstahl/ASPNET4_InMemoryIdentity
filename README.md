# ASPNET4_InMemoryIdentity

## UnitTests

1. Get yourself a working Cassandra instance with "cassandra" pointing to your cassandra IP.

	i.e.
		ContactPoints =  {"cassandra"},
        Credentials =  {Password = "", UserName = ""},
        KeySpace = "identityserver3"

		I use this one.
		https://github.com/bcantoni/vagrant-cassandra/tree/master/1.Base
		

2. Setup your keyspaces

		CREATE KEYSPACE IF NOT EXISTS notforproduction
		WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};

		CREATE KEYSPACE IF NOT EXISTS identityserver3
		WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};

3. Configure conf/cassandra.yaml so you can connect to it from outside the VM  

		# listen_address: localhost
		listen_interface: eth0
		# listen_interface_prefer_ipv6: false

		# rpc_address: localhost
		rpc_interface: eth1
		# rpc_interface_prefer_ipv6: false


4. Run the unit tests.
