local util=require 'xlua.util'

function start()
	local query = "select steam_id,user_name from players_basic limit 50";
    CS.SQLRequestAgent.Instance:PerformSQLRequest(query,callback)
	self:StartCoroutine(util.cs_generator(query_routine))
end

function callback(reader)
	while(reader:Read()) do
        local steamId = reader:GetString("steam_id")
        local userName = reader:GetString("user_name")
        print("SteamID:"..steamId..",UserName:"..userName)
	end
end

function query_routine()
	while true do
		print("Test")
		coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
	end
end