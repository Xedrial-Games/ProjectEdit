Transform = {};
TransformMt = {};

local function new(entity)
    return setmetatable({ entity = entity }, TransformMt);
end

function Transform.new(entity)
    return new(entity);
end

function TransformMt.__index(obj, index)
    if (index == "position") then
        return vec3.new(InternalCalls.GetTranslation(obj.entity.entityHandle));
    elseif (index == "rotation") then
        return quat.new(InternalCalls.GetRotation(obj.entity.entityHandle));
    end

    return nil;
end

function TransformMt.__newindex(obj, index, val)
    if (index == "position") then
        InternalCalls.SetTranslation(obj.entity.entityHandle, val);
    elseif (index == "rotation") then
        return InternalCalls.SetRotation(obj.entity.entityHandle, val);
    end

    return nil;
end

return setmetatable({}, TransformMt);
