SpriteRenderer = {};
SpriteRendererMt = {};

local function new(entity)
    return setmetatable({ entity = entity }, SpriteRendererMt);
end

function SpriteRenderer.new(entity)
    return new(entity);
end

function SpriteRenderer.ToString()
    return "SpriteRenderer";
end

function SpriteRendererMt.__index(obj, index)
    if (index == "color") then
        return color.new(InternalCalls.SpriteRenderer_GetColor(obj.entity.entityHandle));
    end

    return nil;
end

function SpriteRendererMt.__newindex(obj, index, val)
    if (index == "color") then
        InternalCalls.SpriteRenderer_SetColor(obj.entity.entityHandle, val);
    end

    return nil;
end

return setmetatable({}, SpriteRendererMt);
