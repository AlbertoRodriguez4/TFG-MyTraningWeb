import os, re, json

used_keys = set()
for root, dirs, files in os.walk('Frontend/src'):
    for f in files:
        if f.endswith(('.vue', '.ts')):
            path = os.path.join(root, f)
            try:
                content = open(path, 'r', encoding='utf-8').read()
                for m in re.findall(r"\$t\((['\"])(.+?)\1\)", content):
                    used_keys.add(m[1])
                for m in re.findall(r"t\((['\"])(.+?)\1\)", content):
                    used_keys.add(m[1])
            except Exception as e:
                print('Error', path, e)

es = json.load(open('Frontend/src/i18n/es.json', 'r', encoding='utf-8'))
en = json.load(open('Frontend/src/i18n/en.json', 'r', encoding='utf-8'))

missing_es = sorted([k for k in used_keys if k not in es])
missing_en = sorted([k for k in used_keys if k not in en])

print('=== TOTAL USADAS:', len(used_keys), '===')
print('=== FALTAN EN ES:', len(missing_es), '===')
for k in missing_es:
    print(k)
print()
print('=== FALTAN EN EN:', len(missing_en), '===')
for k in missing_en:
    print(k)
