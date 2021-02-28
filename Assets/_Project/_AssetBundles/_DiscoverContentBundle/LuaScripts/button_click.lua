function start()
    self:GetComponent("Button").onClick:AddListener(
        function ()
            CS.SQLRequestAgent.Instance:Toast("Button Click")
        end
    )
end