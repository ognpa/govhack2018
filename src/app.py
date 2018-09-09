#!flask/bin/python
from flask import Flask
import pandas as pd
import requests
from flask import send_file
import matplotlib
matplotlib.use("agg")
from flask import jsonify
import seaborn as sns
import matplotlib.pyplot as plt
#%matplotlib inline
app = Flask(__name__)


all_data_sets={'ato':'ato.xlsx',
               'postcodes':'postcodes.csv'
               
              }



@app.route('/ato/sz')
def atoSize():
    #ato=pd.read_excel('ato.xlsx',sheet_name='ATO Data')
    return str(ato.shape[0])

@app.route('/ato/cols')
def atoCols():
    #ato=pd.read_excel('ato.xlsx',sheet_name='ATO Data')
    dataset_list = '  ,'.join(ato.columns)
    dataset=[]
    for i in ato.columns:
        dataset.append(i)
    return jsonify(dataset)
    
    


@app.route('/ato/dtypes')
def atoDtypes():
    #ato=pd.read_excel('ato.xlsx',sheet_name='ATO Data')
    dtype_dict={}
    for i in ato.columns:
        dtype_dict[i]=str(ato[i].dtype)
    
    return jsonify(dtype_dict)


@app.route('/list_all')
def listAllSupported():
    return jsonify(list(all_data_sets.keys()))

#@app.route('/all')
#def allDatasets():
#   #https://search.data.gov.au/search?q=electricity
#    r = requests.get('https://search.data.gov.au/search?q=electricity')
#    return r.text


@app.route('/ato/graph2.0/<name>')
def atoGraph2View(name):
    return send_file(name+".png", mimetype='image/png')



@app.route('/ato/graph2.0')
@app.route('/ato/graph2.0/')
def atoGraph2():
   all_graphs=[]
   plt.figure(1)
   sns.countplot(ato['Income year']) 
   plt.savefig("1.png")
   plt.show()
    
   plt.figure(2)
   sns.pointplot(x="Income year", y="Net tax", data=ato)
   plt.savefig("3.png")
   plt.show()

    

   all_graphs=['1.png','2.png']
    
   return str(len(all_graphs))

@app.route('/ato/graph')
def atoGraph():
   # ato=pd.read_excel('ato.xlsx',sheet_name='ATO Data')
    g=sns.countplot(ato['Income year']).get_figure()
    g.savefig("output.png")
    return send_file("output.png", mimetype='image/png')



@app.route('/')
def index():
   # ato=pd.read_excel('ato.xlsx',sheet_name='ATO Data')
    return "hello world"


if __name__ == "__main__":
    ato=pd.read_excel('ato.xlsx',sheet_name='ATO Data')

    app.run(host="0.0.0.0", port=80)
    
