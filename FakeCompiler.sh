KC_PATH="$HOME/.local/share/Steam/steamapps/common/Kingdoms and Castles/KingdomsAndCastles_Data/mods"
KR_PATH="$KC_PATH/KingdomRenderer"
SRC_PATH="./KingdomRenderer/Source"

rm -r "$KR_PATH"
cp -r "$SRC_PATH" "$KR_PATH"

# Delete build log
rm "$KC_PATH/log.txt"

xdg-open steam://run/569480