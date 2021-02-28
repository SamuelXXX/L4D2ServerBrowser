local util=require 'xlua.util'
local sql_agent=CS.SQLRequestAgent.Instance

function start()
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
	local query = "select steam_id,user_name from players_basic limit 50";
	while true do
		sql_agent:Toast("SQL query")
		sql_agent:PerformSQLRequest(query,callback)
		coroutine.yield(CS.UnityEngine.WaitForSeconds(10))
	end
end