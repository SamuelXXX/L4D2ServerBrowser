function start()
	print("start")
	local query = "select steam_id,user_name from players_basic limit 50";
    CS.SQLRequestAgent.Instance:PerformSQLRequest(query,callback)
end

function callback(reader)
	print("callback")
	while(reader:Read()) do
        local steamId = reader:GetString("steam_id")
        local userName = reader:GetString("user_name")
        print("SteamID:"..steamId..",UserName:"..userName)
	end
end
